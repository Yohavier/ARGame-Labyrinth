# Penumbra

**Content**
- [Ablauf](#ablauf)
	- [Sieg-Alien](#sieg-Alien)
	- [Sieg-Crew](#sieg-Crew)
- [Spielregeln](#spielregeln)
	- [Allgemeine-Regeln](#allgemeine-Regeln)
	- [Alienregeln](#alienzusatzregeln)
		- [Alien-Klassen](#alien-Klassen)
	- [Crewregeln](#crewregeln)
		- [Crew-Klassen](#crew-Klassen)
- [Menu](#menu)
	- [Join](#join)
	- [Lobby](#lobby)
	- [HUD](#hUD)
- [AR-Marker](#aR-Marker)
	- [Board-Marker](#board-Marker)
	- [Lobby-Marker](#lobby-Marker)
	- [Tile-Marker](#tile-Marker)
- [Items](#items)
	- [Terminal](#terminal)
- [Power-Ups](#power-Ups)
	- [Funkgerät](#funkgerät)
	- [Medipack](#medipack)
	- [Türhack](#türhack)
	- [Scanner](#scanner)
	- [Shutdown](#shutdown)

## Ablauf
Es gibt Insgesamt 4 Spieler. 1 Alien und eine Crew von Insgesamt 3 Mitgliedern.
### Sieg-Alien
Damit das Alien gewinnt, muss es nur alle Crewmitglieder töten, bevor die Rettungskapseln
von der Crew repariert wurden.
### Sieg-Crew
Die Crew gewinnt, wenn sie es schaffen alle nötigen Items auf dem Schiff zu sammeln und diese
zur Rettungskapsel zu bringen, sowie den Terminal zu aktivieren. Die Crew gewinnt auch, wenn nur
einer überlebt. 

## Spielregeln

### Spielregeln
Jeder Spieler würfelt zum Beginn seines Zuges und kriegt anschließend angezeigt wie viele Schritte ihm zur verfügung stehen. Der Spieler kann
seine Schritte bei bedarf auch Stück für Stück aufbrauchen.
Je nachdem welche Klasse man gewählt hat kann diese Zahl noch entsprechend moduliert werden. Anschließend hat der Spieler zu jeder
Zeit in seinem Zug die möglichkeit das im All schwebende Raum-Tile wieder in die Raumstation zu schieben. Wie genau dies funktioniert wird
genauer im [Tile-Marker](#tile-Marker) Abschnitt erklärt. Der Spieler muss nicht zwingend alle Schritte gehen, kann diese sich allerdings 
auch nicht für seinen nächsten Zug aufbewahren.
		
### Alienregeln
Das Alien hat keine weiteren Möglichkeiten, außer das es automatisch Spieler töten, wenn diese sich im gleichen Raum befinden.

#### Alien-Klassen

### Crewregeln
Ein Crewmitglied kann jederzeit, während seinem Zug ein PowerUp verwenden oder Item einsammeln. Mehr dazu in [Items&PowerUps(#items&PowerUps).
Zudem ist es nicht möglich für ein Crewmitglied einen Raum zu durchqueren, in dem bereits ein Spieler ist. 

#### Crew-Klassen
##### Standart
##### Juggernaut
##### Scout
##### Runner
##### Mechaniker

## Menu

### Join
Hier muss man die IP-Adresse des Host eingeben um sich zu verbinden. Der Host ist immer das Alien.
### Lobby
Um die Characterauswahl durchführen zu können, muss man den [Lobby-Marker](#lobby-Marker) tracken und durch den "Select"-Button
die Luke des Raumschiffs öffnen. Dann kann man durch swipen sich einen Character aussuchen.
Wenn man fertig ist muss man auf "Ready" klicken. Wenn alle "Ready" sind, kann der Host das Spiel starten.
### HUD
Im HUD wird folgendes angezeigt:
- welcher Spieler dran ist
- wie viele Schritte man noch tätigen kann
- welches Item man im Inventar hat
- welche Power-Ups und interaktionsmöglichkeiten man hat
- der Button zum würfeln
- wie der Gesundheitszustand des Spielers ist
- ein Pfeil um seinen Zug zu beenden


## AR-Marker
### Board-Marker
Diese Marker dienen dazu das Spielbrett zu tracken. Dafür ist allerdings nur einer nötig.
Jeder von den vieren setzt das Spielbrett an die korrekte position.

<p float="left">
  <img width="100" src="DVL/Assets/ImageLibrary/BottomLeft.jpeg">
  <img width="100" src="DVL/Assets/ImageLibrary/BottomRight.jpeg">
  <img width="100" src="DVL/Assets/ImageLibrary/Top.png">
  <img width="100" src="DVL/Assets/ImageLibrary/TopRight.jpeg">
</p>

### Lobby-Marker
Dieser Marker wird nur einmal zu beginn benötigt, damit jeder Spieler sich eine Lobby an seinen Platz
spawnen kann. Dieser wird nach der Characterselection nicht mehr benötigt.

<img width="100" src="DVL/Assets/ImageLibrary/Lobby.jpeg">

### Tile-Marker
Der Marker trackt das Raum-Tile, welches rausgeschoben wurde und die Spieler einmal in ihrem Zug
wieder ins Brett schieben können.

<img width="100" src="DVL/Assets/ImageLibrary/TopLeft.jpeg">


## Items
Gleichzeitig kann ein Crewmitglied 2 PowerUps und 1 Item im Inventar haben. 

Um Items aufzusammeln, muss man sich im gleichen Feld befinden und auf dieses klicken. Dann geht eine kleine UI auf, auf welcher weiter Anweisungen stehen.
#### Terminal
Der Terminal ist ein spezielles Item und erfordert mehrere Runden um fertig konfiguriert zu werden.


## Power-Ups
Power Ups kann man mithilfe des HUD einsammeln. Wenn man die Möglichkeit hat eins aufzusammeln/auszutauschen, wird dies im HUD sichtbar.
#### Funkgerät
Zeigt die Sicht eines zufälligen anderen verbündeten Spielers an und hält für 8 Runden.
#### Medipack
Gibt die Möglichkeit einen anderen Spieler der im State: Dying ist zu heilen.
#### Türhack
Öffnet eine Tür.
#### Scanner
Erweitert die Sichtweite des Spielers um +1 für 8 Runden.
#### Shutdown
Schließt alle Türen und mischt das Spielfeld einmal neu.


