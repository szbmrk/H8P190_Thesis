# H8P190 Thesis for Computer Science BSc at Eszterhazy Karoly Catholic University

The objective of my thesis is to develop a poker game based on Texas Hold'Em rules, where a desktop computer is connected to several mobile phones to implement a multiplayer game. The game is designed to give players a similar experience as playing a real poker game in real life. This was achieved by using the mobile phones as the "hands" of the players and the computer as the table, the common space.

## Architectural design

The thesis is composed of 3 different projects. Since all three projects use C# programming language, it is easy to manage common methods, libraries, functions.

### PC game

The PC game serves as the common place, where games can be created and managed. The important parts of the gameplay can be tracked here for the players.

### Mobile game

The role of the Mobile game is to create a communication with the PC game for the players. Here players can play the game by sending various messages, while they can follow the rest of the gameplay on the PC.

### Shared DLL

The role of the Shared DLL is to collect all the functions and methods that I use in the code for both the PC and the Mobile game. One of the most important parts of this DLL is the algorithm for hand evaluation. Another important part of the DLL is the unification of messages between the two games.