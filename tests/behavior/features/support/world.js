const {setWorldConstructor, setDefaultTimeout} = require('cucumber');
const {MongoClient} = require('mongodb')

/**
 * Gets a value from the environment or returns the default.
 *
 * @param {string} varname
 *   The key of the variable in the environment.
 * @param {object} defaultvalue
 *   The value to use if the key is not present in the environment.
 * @returns {object}
 *   The value referenced by the varname or the default provided.
 */
let getEnvironmentVar = function (varname, defaultvalue)
{
    var result = process.env[varname];
    if (result != undefined){
        // console.debug('ENV VAR used. ' + varname + ' ' + result)
        return result;
    } else {
        // console.debug('DEFAULT used. ' + varname + ' ' + defaultvalue)
        return defaultvalue;
    }
}

function CustomWorld() {
    let parent = this

    /**
     * Builds a "v1" api uri to aid in testing.
     *
     * @param {string} part
     *   The variable section of the uri
     * @returns {string}
     *   A full formed uri.
     */
    this.v1Url = function(part) {
        var url = this.rootUrl + '/api/v1'
        if (!part.startsWith('/')){
            url += '/'
        }
        return url + part
    }

    /**
     * Sleeps the current thread for the provided milliseconds.
     *
     * @param {int} ms
     *   The milliseconds to sleep
     */
    this.sleep = function (ms) {
        return new Promise((resolve) => setTimeout(resolve, ms));
    }

    this.getEnvironmentVar = getEnvironmentVar

    /**
     * Polls the database invoking the completed callback when the job has been detected as complete or when the
     * timeout has elapsed
     *
     * @param {string} jobUuid
     *   The job uuid in the database that is checked for completing.
     * @param {function} completed
     *   The function that will be called when the job has been detected as completed.
     *   format: function(test, job, callback)
     *     param {object} test - a reference to the test object.
     *     param {object} job - a reference to the job object loaded from the database.
     *     param {function} callback - a reference to the cucumber callback used to signal this test is complete.
     * @param {int} timeout
     *   The maximum amount of time to wait for a job to complete. When the timeout has elapsed the current state of the job will be returned.
     */
    this.waitForJobToComplete = function (jobUuid, timeout = 30000) {
        let timeoutTs = new Date().getTime() + timeout

        return new Promise((resolve, reject) => {
            let work = function () {
                try {
                    MongoClient.connect(parent.mongoUrl, function(err, db) {
                        try{
                            if (err) reject(err)
                            let dbo = db.db(parent.appDb)
                            let dbc = dbo.collection('jobs')
                            let query = { uuid: jobUuid }

                            dbc.find(query).toArray(function(err, result){
                                if (err) throw err;
                                if (result.length != 1) {
                                    throw "More than one job found for job id: " + jobUuid
                                }

                                let item = result[0];
                                var awaitStatus = ['Successful', 'Failed']
                                if (new Date().getTime() > timeoutTs){
                                    resolve(item)
                                } else if (awaitStatus.indexOf(item['status']) > -1) {
                                    resolve(item)
                                } else {
                                    // NOTE: We have to sleep here so Mongo's eventual consistency has the
                                    // new job available for reading.
                                    parent.sleep(1000).then(() => work())
                                }
                            })
                        }
                        catch(err){
                            reject(err);
                        }
                        finally{
                            db.close()
                        }
                    });
                }
                catch(err){
                    reject(err);
                }
            }

            work()
        })
    }

    this.rootUrl = this.getEnvironmentVar('API_URL', 'http://test-api:5000')
    this.mongoUrl = this.getEnvironmentVar('MONGO_URL', 'mongodb://test-mongo:27017/auth')
    this.appDb = this.getEnvironmentVar('MONGO_DATABASE', 'corp-hq')
}

setWorldConstructor(CustomWorld)
setDefaultTimeout(parseInt(getEnvironmentVar('CORPHQ_BEHAVE_STEP_TIMEOUT', '5')) * 1000)