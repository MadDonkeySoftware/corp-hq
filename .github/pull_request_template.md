### Story
https://maddonkeysoftware.myjetbrains.com/youtrack/issue/CHQ-1

#### Depends On (Optional)
https://github.com/MadDonkeySoftware/corp-hq/pull/1

### Summary
* Moved `File1` from API to Common project as it is now shared by API and Runner
* Implemented background task monitor
* Etc. These are semi-high level items that assist reviewers by giving them additional context while reviewing your PR

### Testing
* Here is where you list any additional manual testing instructions to reviewers. 
* Start API and Runner project
* `curl -X POST http://127.0.0.1:5000/api/v1/job -H 'key: value' -d '{ "bodyKey": "bodyValue" }'`
* Verify job id returned. Verify runner processes job successfully. 
* `curl -X POST http://127.0.0.1:5000/api/v1/job -H 'key: value' -d '{ "badKey": "badValue" }'`
* Verify job id returned. Verify runner marks job failed and logs failure appropriately.
  * Job failure can be seen by querying mongo: `db.jobs.find({ "uuid": "[id here]" })`
