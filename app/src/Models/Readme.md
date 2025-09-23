# What goes here?

This project should only contain the models which should be saved in the database layer.  
This is not responsible for the database access itself, and should not have any references to anything EntityFramework related.  
All relationsships should be initialized in the entity, to avoid null references in the code.  