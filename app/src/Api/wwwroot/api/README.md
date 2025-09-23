The Quartz database must be in GIT, but we dont want to commit any changes to it.  

To avoid changes to the QuartzDatabase run the following command.

```
    git update-index --skip-worktree QuartzDatabase.db
```

Need to create a new QuartzDatabase.db file?  
Do the following:  

Download the SQLITE database script from: https://github.com/quartznet/quartznet/blob/main/database/tables/tables_sqlite.sql  
Save the file in the `wwwroot/api` folder, filename doesn't matter.   
Run the following command
```
    cat sqlFileName.sql | sqlite3 QuartzDatabase.db
```
Add the file to GIT  
Now run the command 

```
    git update-index --skip-worktree QuartzDatabase.db
```

To ignore it.