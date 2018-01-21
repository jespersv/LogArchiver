# LogArchiver
Simple program for archiving log files to 7z archives

## Config example
Example for **archives.json**
Explanation:
* archives - is the list of different locations that is to be archives into 7z-archive files.
* filePath - the path where the logs are{
* archivePath - where to store 7z-archive files
* archiveName - the name of the 7z-archivefile. note that .7z ext will be added as well as todays date. and depending on if that filename exists it will add a trailing number iterating up. eg. "[machineName]_[archiveName]_[todaysDate(yyyyMMdd)].7z"
* deleteArchivedFiles - should mostly likly always be set to true, only ever applicable for testing purposes. ''todo: default this value to false''

eg.
```
{
  "archives": [
    {
      "filePath": "E:\\WEBLogFiles\\W3SVC6",
      "archivePath": "E:\\WEBLogFilesArchive\\W3SVC6_site_com",
      "archiveName": "site_com",
      "deleteArchivedFiles": "true"
    },
    {
      "filePath": "E:\\Logs",
      "archivePath": "E:\\Logs\\archive",
      "archiveName": "applicationLog4netLogs",
      "deleteArchivedFiles": "true"
    }
  ]
}
```
