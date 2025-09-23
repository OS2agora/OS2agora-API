# Postman test suite
Test collections that can be run in postman.

## How to run the tests
1. Import all the files in this folder into a Postman workspace.
2. Modify the environment variables to target the environments you want to test.
3. Run the collection. (The order of the requests in the collection matters, but when running the collection, Postman
   runs the requests in the specified order.)

## How to run test as a logged in user
Add the session cookie to the Postman environment variables  `sessionCookieOld` and `sessionCookieNew` where the former 
is the session cookie for the old API and the latter is the session cookie against the new API. Make sure the x-api-key 
variable correspond to what was used to create the session.
