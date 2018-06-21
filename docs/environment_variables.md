## UI
| Key            | Default               | Description |
|----------------|-----------------------|-------------|
|                |                       |             |

## API
| Key                    | Default    | Description |
|------------------------|------------|-------------|
| ASPNETCORE_ENVIRONMENT | Production | If "Development", the developer exception page is returned on 500's |
| MONGO_CONNECTION       | null       | The mongo database in which most application settings are stored. |
| MONGO_DATABASE         | corp-hq    | The collection to use when persisting information in mongo. |
| CORPHQ_API_URL         | null       | Used as the base for all generated api calls to the backend. |
| CORP_HQ_ENVIRONMENT    | null       | Used to refine searches for settings in the event that multiple environments share a DB. This is most commonly done during local development. |
|                        |            |             |

## Runner
| Key                 | Default | Description |
|---------------------|---------|-------------|
| MONGO_CONNECTION    | null    | The mongo database in which most application settings are stored. |
| CORP_HQ_ENVIRONMENT | null    | Used to refine searches for settings in the event that multiple environments share a DB. This is most commonly done during local development. |
|                     |         |             |

## Celery Tests
| Key                        | Default                        | Description |
|----------------------------|--------------------------------|-------------|
| CORPHQ_BEHAVE_SLEEP        | 2                              | Time to sleep between certain steps in the `behave-ci-cd.sh` script. |
| CORPHQ_BEHAVE_STEP_TIMEOUT | 5                              | Time for a cucumber-js step to timeout. |
| API_URL                    | http://localhost:5000          | The URL to make requests to when testing. |
| MONGO_URL                  | mongodb://127.0.0.1:27017/auth | The mongo db endpoint to interact with during testing. Tis is to seed / verify data of tests. |
| MONGO_DATABASE             | corp-hq                        | The default collection to use while executing tests. |
|                            |                                |             |