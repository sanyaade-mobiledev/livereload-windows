@call PubConfig.cmd
%~dp0tools\S3Sync\S3Sync.exe -AWSAccessKeyId %AWSAccessKeyId% -AWSSecretAccessKey %AWSSecretAccessKey% -SyncDirection upload -LocalFolderPath "bin\Debug\app.publish\Application Files" -BucketName download.livereload.com -S3FolderKeyName "windows/Application Files/" -UploadHeaders x-amz-acl:public-read -DeleteS3ItemsWhereNotInLocalList false -UseSSL false -TransferThreads 5 -MultipartThreads 1
