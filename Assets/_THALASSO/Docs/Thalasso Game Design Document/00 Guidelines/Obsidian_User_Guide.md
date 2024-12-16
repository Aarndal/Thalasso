---
aliases:
  - Markdown
  - Formatierung
tags:
---
#UserGuide 
# Grundlegende Formatierungsoptionen
---
Obsidian funktioniert nach dem Markdown-System und verwendet **Sonderzeichen** um Teile des Textes oder Dokumentes auf unterschiedliche Weisen zu formatieren.

## Basic Textformatierung
---
**Überschriften** werden durch **#** erstellt, wobei zusätzliche **#** weitere untergeordnete Unterschriften markieren.

**Fetter Text wird mit von \*\* eingerahmt**
*Kursiver Text wird von \* eingerahmt*
***Kursiv und fetter Text wird von *** eingerahmt***
~~Durchgestrichener Text wird von ~~ (zwei Tilden) eingerahmt~~
==Farblich hervorgehobener Text wird von \=\= eingerahmt==
> Eingeschobene Zitate werden durch ein > am Anfang der Zeile erstellt


## Callouts
---
Callouts können verwendet werden um Teile des Textes besonders hervorzuheben, FAQs zu erstellen, Belege oder Links leicht kenntlich einzufügen oder um Dinge wie Aufgabenlisten leichter zu strukturieren.

>[!info]- INFO
>Info-Boxen werden erstellt mit: **>[!info]**

>[!note]- NOTE
>Notiz-Boxen werden erstellt mit: **>[!note]**

>[!todo]- TO-DO
>To-Do-Boxen werden erstellt mit: **>[!todo]**

>[!tip]- TIP
>Tipp-Boxen werden erstellt mit: **>[!tip]**, **>[!hint]** oder **>[!important]**

>[!success]- CHECK
>Check-Boxen werden erstellt mit: **>[!success]**, **>[!check]** oder **>[!done]**

> [!question]- QUESTION
> FAQ-Boxen werden erstellt mit: **\>\[!question]**, **>[!help]** oder **>[!faq]**

>[!warning]- WARNING
>Warnungs-Boxen werden erstellt mit: **>[!warning]**, **>[!caution]** oder **>[!attention]**

>[!failure]- FAILURE
>Fail-Boxen werden erstellt mit: **>[!failure]**, **>[!fail]** oder **>[!missing]**

>[!danger]- DANGER
>Danger-Boxen werden erstellt mit: **>[!danger]** oder **>[!error]**

>[!bug]- BUG
>Bug-Boxen werden erstellt mit: **>[!bug]**

>[!example]- EXAMPLE
>Beispiel-Boxen werden erstellt mit: **>[!example]**

>[!quote]- QUOTE
>Zitat-Boxen werden erstellt mit: **>[!quote]** oder **>[!cite]**

>[!note] Callouts einklappen
>Callouts können eingeklappt werden, wenn hinter die ursprüngliche Bezeichnung (**>[!]**) ein **-** gesetzt wird.

>[!example] Callouts nesten
>Callouts können ineinander genestet werden, indem vor der ursprünglichen Bezeichnung (**>[!]**) zusätzliche **>** für jede Nestung gesetzt werden. 
>Genestete Callouts können ebenfalls einklappbar gemacht werden.
>>[!check] Beispiel A **>>[!check]** 
>>Matroschka lässt grüßen.
>>>[!check]- Beispiel B **>>>[!check]**
>>>Hab einen Keks!

## Listen
---
Geordnete Listen  werden durch **Zahlen** erstellt (1., 2., 3., etc)

	1. Hier
		1.1. ein
	2. Beispiel


Ungeordnete Listen werden durch **-**, **+** oder **\*** am Anfang der Zeile erstellt

- Hier 
	+ ein
		* Beispiel


Checklisten werden durch **- \[ ]** am Anfang der Zeile erstellt

- [ ] Hier
	- [ ] ein
		- [ ] Beispiel


## Tabellen
---
**Tabellen** können mit **|** und **-** erstellt werden.

Markdown:

\| Name \| Beschreibung |
\|-\|-|
\|Alpha\| Erster griechischer Buchstabe|
\|Beta\| Zweiter griechischer Buchstabe| 

Ergebnis:

| Name | Beschreibung |
| -|-|
|$$\alpha$$| Erster griechischer Buchstabe|
|$$\beta$$| Zweiter griechischer Buchstabe|

>[!info]- Synonyme in Tabellen
>Wird eine Zelle mit **\[\[Name eines verlinkten Dokuments** + **\\** + **alternative Beschreibung]]** gefüllt, lässt sich ein Dokument unter anderem Namen eintragen.

## Fußnoten
---
**Fußnoten** werden mithilfe als **\[^*Zahl der Fußnote*]** im Text markiert. [^1] *<Click Me*
>[!warning] Verlinkungen zu Fußnoten funktionieren nur im Lesemodus! Im Schreibmodus sind diese deaktiviert.

[^1]: Die Fußnote wird am Ende des Abschnittes mit der **gleichen** Markierung **\[^*Zahl der Fußnote*]:** niedergeschrieben. (den **:** nicht vergessen, sonst verlinkt die Fußnote nicht)

>[!info]- Inline Fußnoten
>Fußnoten können im Fließtext verfasst werden, indem sie anstelle von **\[^Zahl]** als **\^\[Fußnotentext]** notiert werden. Inline Fußnoten werden trotzdem mit der jeweiligen Ziffer nummeriert, brauchen aber keinen separaten Abschnitt um runtergeschrieben zu werden.
## Verlinken und Einbetten von Medien
---
Medien wie Fotos, Videos oder Tweets können durch **\[*Anzeigename*]\(*Link*)** eingefügt werden.
Durch das hinzufügen eines **!** am Anfang, wird das Medium direkt im Dokument eingebettet.

>[!tip]- Verlinkung von Obsidian-Internen Medien
>Um Screenshots, Videos, o.Ä., welche bereits in Obsidian abgespeichert wurden, in Dokumenten zu verlinken, können diese aus der Ordnerstruktur per **Drag-and-Drop** oder mithilfe von **\!\[\[Dateiname.Dateiendung]]** eingebettet werden. 
>
>Dateien können auf deren Eintrag im Wiki umbenannt werden, wobei auch alle Verlinkungen und deren Bezeichnung geupdatet werden.

Beispiel 
[Obsidian Tutorial](https://www.youtube.com/watch?v=5qRVquqRH0k)
![Obisidan Tutorial](https://www.youtube.com/watch?v=5qRVquqRH0k)

|Typ|Unterstütze Formate|
|-|-|
|Markdown|md|
|Bilder|png, webp, jpg, jpeg, gif, bmp, svg|
|Audio|mp3, webm, wav, m4a, ogg, 3gp, flac|
|Video|mp4, webm, ogv, mov, mkv|
|PDF|pdf|
## Miscellaneous
---
Trennstriche innerhalb eines Dokumentes werden mit **---** erstellt.

# Obsidian Funktionalität
---
Die folgenden Funktionen sind exklusiv zu Obsidian und nicht Teil von Markdown. Manche Funktionen wie Verlinkungen wie Verlinkungen existieren zwar in Markdown, jedoch kann deren Syntax und Umsetzung leicht bis stark abweichen.
## Verlinkungen
---
Verlinkungen stellen das Haupt-Alleinstellungsmerkmal von Obsidian dar.
Sie werden verwendet um Notizen und Artikel leicht und schnell zu strukturieren und Zusammenhänge zwischen verschiedenen Einträgen zu visualisieren und nutzbar zu machen.

Werden von Obsidian dieselben Stichpunkte in zwei oder mehr Dokumenten erkannt, schlägt es im **Link-Reiter**, in der rechten, oberen Ecke, eine mögliche Verbindung vor. Wird dieser bestätigt, erstellt Obsidian automatisch eine **Verlinkung**, welche selbst bei Umbenennung des Stichwortes oder Zielartikels bestehen bleibt. 

>[!info]- Ein- oder Ausgehende Verlinkungen
> Es gibt zwei unterschiedliche Reiter für eingehende und ausgehende Verlinkungen, es macht also Sinn, jeweils beide auf Dokumenten zu bestätigen um zu überprüfen ob mögliche Verlinkungen einer der beiden Richtungen erstellt werden können.

>[!warning]- Synonyme und Aliases
>Standardmäßig zielt die Stichworterkennung nur auf den Namen von existierenden Dokumenten ab und möchte sich mit diesen verlinken sofern das Stichwort und der Dokumentenname identisch sind.
>
>Dokumentnamen können mit Synonymen versehen werden, wenn direkt anschließen an die Überschrift ein **---** gesetzt wird und im erscheinenden Dropdrown **aliases** ausgewählt wird.
 ^04d1b2

Verlinkungen können händisch erstellt werden, wenn im Fließtext ein **\[\[** eingesetzt wird.
Dabei wird ein **Dropdown-Menü** aufgerufen, welches existierende Dokumente auflistet und suchbar macht.

Wird eine Verlinkung ohne gültiges Ziel **\[\[*Name eines nicht-existenten Dokumentes*]]** aufgesetzt, wird bei dem Anklicken dieser Verlinkung automatisch ein neues Dokument mit dem entsprechenden Namen und Verlinkung erstellt.

### Überschriften verlinken
---

Untergeordnete Überschriften lassen ebenfalls mit Verlinkungen anzielen, indem zusätzlich zum Dokumentnamen die gewünschte Überschrift mit einer **#** anhängen. 
**\[\[Dokument\#Überschrift]]** 
 ^ueberschriften-verlinken-infotext


>[!info] Sobald die **#** eingefügt wurde, öffnet sich ebenfalls ein Dropdown, welches dich zwischen den verschiedenen Überschriften auswählen lässt. 

### Blöcke verlinken
---
Blöcke (Paragraphen, Callouts, Listen, etc.) können ebenfalls verlinkt werden.
Dafür wird in der Klammer ein **^** gesetzt, wodurch ein **Dropdown** geöffnet wird, welches jeden Block des Dokuments auflistet und zur Verlinkung auswählbar macht.
**\[\[#^04d1b2]]** = [[#^04d1b2]] 

>[!tip] Blockverlinkungen mit alternativem Text versehen
>Blockverlinkungen können nach dem Identifier mit einem **|** und **Text** versehen werden, um diesen Text im Fließtext als klickbare Verlinkung, anstelle des generierten Identifiers, anzeigen zu lassen.
>**\[\[#^04d1b2|Synonyme]]** = [[#^04d1b2|Synonyme]] 

>[!info]- Lesbare Blockverlinkungen
>	Lesbare Blockverlinkungen können erstellt werden, wenn unter dem Zielblock manuell ein lesbarer Identifier gesetzt wird. Dieser besteht generell aus einem **^**  und Text, wobei Leerzeichen durch **-** ersetzt wird. 
>	**\[\[#^ueberschriften-verlinken-infotext]]** = [[#^ueberschriften-verlinken-infotext]]
>>[!warning] Umlaute nach dem **^** sorgen dafür, dass die zuerst gesetzte Textmarkierung nicht funktioniert. 
>>**Ebenfalls muss der Identifier anschließend an den Block in der nächsten Zeile mit einem vorhergehendem Leerzeichen notiert werden.**

>[!tip]- Blöcke in anderen Dokumenten verlinken
>Blöcke aus anderen Dokumenten können durch das Eingeben von **\[\[^^]]** durchsucht werden.
>Blöcke können ebenfalls durch den Syntax von **\[\[Dokument#^Identifier]]** direkt angesteuert werden.
## Tagging
---
Dokumente können ebenfalls mit **Hashtags** versehen werden um sie neben der Überschrift weiter zu kategorisieren und suchbar zu machen. Dazu kann in einem beliebigen Punkt des Dokumentes eine **#** + **Tag** eingesetzt werden.

Tags können ebenfalls am Kopf des Dokumentes wie [[#^04d1b2|Synoyme]] hinzugefügt werden.

>[!warning] Tags müssen mindestens einen **Nicht-Ziffer** Zeichen beinhalten! 
>**#2023** ist nicht gültig, während **\#y2023** oder **\#2023_** funktionieren.

>[!tip] Sub-Tags (aka. Nested Tags)
>Tags können weiter mit Sub-Tags unterteilt werden, indem dem Tag ein zweiter Tag mithilfe eines **/** angehangen wird. **\#Tag/Subtag**
>Tags dieser Art tauchen bei Suchen nach dem Haupt-Tag auf, lässt einen jedoch Tags mit anderen Sub-Tags ausschließen, um Suchen zu erleichtern.
# Interface
---
Obsidian ähnelt in seinem Aufbau stark einem klassischen Internet Browser gemischt mit einem Texteditor, wobei gängige Tastenkombinationen oder Shortcuts übernommen sind.
## Tabs
---
Es können mehrere Tabs in Obsidian parallel geöffnet werden.

Zum Öffnen eines Dokuments oder einer verlinkten Datei in einem neuen Tab, kann dieser mit einem **mittleren Mausklick** geöffnet werden.

>[!tip] Multi-Tasking
>Tabs können per **Drag-and-Drop** in einer **geteilten** Ansicht gedockt werden, oder als **neue** Fenster geöffnet werden. Dies ist auch möglich durch einen Rechtsklick auf die einzelnen Tabs und dem Auswählen der **"Split"**-Optionen.

>[!info]- Tabs pinnen
>Tabs können ebenfalls aus dem **Rechtsklickmenü** gepinnt werden, wodurch diese **dauerhaft** geöffnet bleiben und andere Dokumente automatisch in einem neuen Tab geöffnet werden, falls der gepinnte Tab aktiv ist.

## Read-, Editing- und Source View
---
Bei der Nutzung von Obsidian lässt sich grundsätzlich zwischen  den 3 folgenden Ansichten, **oben rechts** innerhalb des Tabs, wechseln:
### Read View
In der Read View lassen sich Dokumente **formatiert** anzeigen, jedoch nicht bearbeiten. Manche Elemente wie Fußnoten oder bestimmte Verlinkungen funktionieren nur in der Read View, weshalb die Read View vor allem beim Nutzen von Obsidian als Nachschlagwerk nützlich ist. 
### Edit View
In der Edit View lassen sich Dokumente formatiert anzeigen und bearbeiten. Text- oder Wortblöcke welche bearbeitet werden, werden als Markdown Text anzeigt und die meisten Elemente, z.B. Verlinkungen oder Einbettungen, sind funktionsfähig.
Diese Ansicht wird meistens beim Erstellen oder Bearbeiten von Dokumenten verwendet.
### Source View
In der Source View wird das Dokument in seiner Markdown-Form dargestellt. Dabei werden alle Formatierungen abgeschaltet (farbliche Änderungen bleiben bestehen), was das Bearbeiten von stark formatierten Elementen erleichtert, z.B. Links, Tabellen oder Infoboxen.




>[!tip]- Multi-Tasking und View Settings
> Das Auswählen von verschiedenen Ansichten bezieht sich immer auf das aktuelle Dokument und wird nicht auf das ganze Programm. So lässt sich das gleiche Dokument in zwei unterschiedlichen Ansichten gleichzeitig öffnen.
# Netiquette?
---
>[!bug] Wird hinzugefügt, sobald relevant

