# Introduction

This API discribes how the internal logic of how Stargazer works.

The project is separated into 2 layers

1. Stargazer - This is the front end of the program
2. DataLayer - This handles communicating with the repository and calculating the locations of the objects

All data is loaded into the DataLayer asynchronously on startup. The front end requests incremental calculations of all objects each frame. By this method, the use is not required to wait when updating the position or time. 