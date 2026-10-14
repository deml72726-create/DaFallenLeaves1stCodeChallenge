# DAKITTY Catcher: Development Report

## Overview
Choice of a mini-game Concept: TheCatcher.
Doing the game using Unity events made the structure so much easier:
-I devided it into minor systems:
-The candies: 2 prefabs both with a simple rigidBody and a spawner with 3 child empty objects for emplacement, the spawner randomly spawns one of the 2 prefabs in one of the 3 child spawners, for the candy which is actually a sushi the player receives 10 points each time he catches one if he miss he loses 10 points, and a second prefab which is a bomb in contact with the player he loses a heart.
-The Player: simple movement system with 2 animations, can only move whithin a restricted area delimited by a game object. 
and a bigger event manager to handles everything ToT.
(voila last minute report writing) 

