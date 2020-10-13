# NEA-Project-Oubliette
 This is Project Oubliette, a CLI-based dungeon-crawler game with automation, increased portability and database support.

# Code
 This game will be written in C#, as this is the programming language I am most familiar with.

## Ordering
 Code will be ordered by access modifier. The order will be as follows: <code>private</code>, <code>protected</code> and <code>public</code>. Static members will be written after non-static members. Body expressions will be used where possible to shorten the code.

## Branches
 The core engine functionality will be written on the <code>main</code> branch, the gameplay will be written on the <code>gameplay</code> branch, database-specific functionality will be written on the <code>database</code> branch, and the level editor functionality will be written on the <code>editor</code>.

# Gameplay
 The gameplay will be played on a top-down grid-based environment called a *map*. Each *map* will have a name, tile and *entity* data, and the date of its creation. An *entity* is a mobile object which can move around in a map and be interacted with by other *entities*.

## Maps
 During runtime, maps will be stored as a two-dimensional array of *tiles*. *Tiles* are individual cells on the grid which not only handle how the cell will be drawn aesthetically, but also contains information which can be used with enemy automation later on.
 
 When saving and loading however, maps will be compressed using a simplified form of Run-Length Encoding and stored as *.map* files. For database support, maps will be separated into their component parts: the name, the tile & entity data, and the date the map was created.

## Entities
 Entities represent both living creatures and inanimate objects. They will be instantiated when a game starts and disposed of when a game ends.

# Databases
 This program will use three databases. One will be for users' accounts, one will be for *author* accounts and the final database will be used for storing *maps*. *Authors* are special user accounts which allow users to upload their own custom maps to the *maps* database.