# CrossesAndZerosAPI

Non-negotiable:

int MoveCode (1 - for cross, 2 - for zero)

USAGE (on the client):

-Call /api/Game/GetStates/{GameID} at the start

It will return string of type "xxxxxxxxx", where x - MoveCode or 0 if square weren't yet assigned

-To make move call /api/Game/MakeMove in Header to which send:

      -Gameid
      -MoveCode
      -Squareposition (Position on the board(check example below))
      
 It will return updated

![Untitled](https://user-images.githubusercontent.com/94042423/224341144-1075f7be-8426-47d0-bf50-ffabd36723be.png)

After recieving new data from /api/Game/MakeMove


How to start:

-Create database using CrossesAndZerosAPIDB.sql

-Change ConnectionString in appsettings.json
