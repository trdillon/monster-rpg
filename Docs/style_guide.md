# RavaR Style Guide

This guide details the guidelines and conventions used in the RavaR project. The goal is to create and maintain a clean, well organized project with consistent
structure, style, formatting and naming.

## Table of Contents
> 1. [Introduction](#introduction)
> 1. [Project Structure](#structure)
> 1. [Scripts](#scripts)
> 1. [Assets](#assets)
> 1. [Conclusion](#conclusion)
> 
<a name="introduction"></a>
## 1. Introduction

### Sections

> 1.1 [Style](#style)

> 1.2 [Important Terminology](#importantterminology)

<a name="style"></a>
### 1.1 Style

#### All structure, assets, and code in the project should look like a single person created it, no matter how many people contributed.

Style guides should be living documents and you should propose style guide changes to this guide if you feel the changes will benefit the project.

Following a style guide also allows for more productive creation and maintenance as one does not need to think about style, simply follow instructions. 
This style guide is written with best practices in mind, meaning that by following this style guide you will also minimize hard to track issues.

<a name="importantterminology"></a>
### 1.2 Important Terminology

<a name="terms-prefab"></a>
#### Prefabs
Unity uses the term Prefab for a system that allows you to create, configure, and store a GameObject complete with all its components, property values, and child GameObjects as a reusable Asset.

<a name="terms-level-map"></a>
#### Levels/Maps/Scene
Levels refer to what some people call Maps or what Unity calls Scenes. A Level contains a collection of objects.

<a name="terms-serializable"></a>
#### Serializable
Variables that are Serializable are shown in the Inspector window in Unity. For more information see Unity's documentation on [Serializable](https://docs.unity3d.com/Manual/script-Serialization.html).

<a name="terms-cases"></a>
#### Cases
There are a few different ways you can name things. Here are some common casing types:

> ##### PascalCase
> Capitalize every word and remove all spaces, e.g. `DesertEagle`, `StyleGuide`, `ASeriesOfWords`.
> 
> ##### camelCase
> The first letter is always lowercase but every following word starts with uppercase, e.g. `desertEagle`, `styleGuide`, `aSeriesOfWords`.
>  ##### lowercase
> All letters are lowercase, e.g. `deserteagle`, 
>
> ##### Snake_case
> Words can arbitrarily start upper or lowercase but words are separated by an underscore, e.g. `desert_Eagle`, `Style_Guide`, `a_Series_of_Words`.

**[⬆ Back to Top](#table-of-contents)**

<a name="structure"></a>
## 2. Project Structure
The directory structure style of a project should be considered law. Asset naming conventions and content directory structure go hand in hand, 
and a violation of either causes unneeded chaos.

In this style, we will be using a structure that relies more on filtering and search abilities of the Project Window for those working with assets 
to find assets of a specific type instead of another common structure that groups asset types with folders.

<pre>
Assets
    <a name="#structure-developers">_Developers</a>(Use a `_`to keep this folder at the top)
        DeveloperName
            (Work in progress assets)
    <a name="structure-top-level">ProjectName</a>
            Characters
            	Gus
            FX
              Beams
                    Beam_Line_Purple_L
            Gameplay
                Characters
                Items
                Monsters
                Moves
                    Prefabs
            <a name="#structure-levels">Levels</a>
                Fornwest
                Region1
                    Level1
            MaterialLibrary
            	Backgrounds
                    Battle
                    Cutscene
            	Terrain
                    Tiles
                      Grass_Light
                      Sand_Tan
            Monsters
                001-Wabbi
                002-Faywi
            Objects
                Architecture (Single use big objects)
                    House
                Props (Repeating objects to fill a level)
                    Fence
                        Wood
            Scripts
                Animation
                Battle
                Character
                    Battler
                    NPC
                Levels
                Monster
                    Condition
                    Move
                UI
            Sound
                FX
                Music
            UI
                Art
                    UI_Dialog_Box
                Resources
                    Font
    ExpansionPack (DLC)
    Plugins
    ThirdPartySDK  
</pre>

The reasons for this structure are listed in the following sub-sections.

### Sections

> 2.1 [Folder Names](#structure-folder-names)

> 2.2 [Top-Level Folders](#structure-top-level)

> 2.3 [Large Assets](#structure-large-assets)

> 2.4 [Scene Structure](#scene-structure)

<a name="2.1"></a>
<a name="structure-folder-names"><a>
### 2.1 Folder Names
These are common rules for naming any folder in the content structure.

<a name="2.1.1"></a>
#### Always Use [PascalCase](#terms-cases)
PascalCase refers to starting a name with a capital letter and then instead of using spaces, every following word also starts with a capital letter. 
For example, `GrassLight` and `SandTan`.

<a name="2.1.2"></a>
#### Never Use Spaces
Never use spaces. Spaces can cause various engineering tools and batch processes to fail. 
Ideally the project's root also contains no spaces and is located somewhere such as `D:\Project` instead of `C:\Users\My Name\My Documents\Unity Projects`.

<a name="2.1.3"></a>
#### Never Use Unicode Characters And Other Symbols
If one of your game characters is named 'Zoë', its folder name should be `Zoe`. Unicode characters can be worse than spaces
for engineering tools and some parts applications don't support Unicode characters in paths either.

Using other characters outside `a-z`, `A-Z`, and `0-9` such as `@`, `-`, `_`, `,`, `*`, and `#` can also lead to unexpected and hard to track issues 
on other platforms, source control, and weaker engineering tools.

#### There is an exception to this rule: Battokuri sprites under `Assets/Ravar/Monsters` are prefixed with XXX- to denote the monster index number.

<a name="structure-no-empty-folders"></a>
#### No Empty Folders
There simply shouldn't be any empty folders. They clutter the content browser.

<a name="2.2"></a>
<a name="structure-top-level"><a>
### 2.2 Use A Top Level Folder For Project Specific Assets
All of the project's assets should exist in a folder named after the project. All of the project's content should exist in `Assets/Ravar`.

> The `_Developers` folder is not for assets that your project relies on and therefore is not project specific. It is used for experimenting and local development.
Assets in the `_Developers` folder should never be included in a build version of the game.

<a name="2.3"></a>
<a name="structure-large-assets"></a>
### 2.7 Large Assets Get Their Own Folder Layout

Tilesets and sprite sheets can produce lots of individual assets, so they deserve their own folders. A tileset with 20 `GrassLightDirt` tiles should be kept in
a separate folder from 40 `GrassLight` tiles to differentiate which set they belong to. When creating a tile palette it's easier to work with the tiles if they are
added in smaller batches with space between them.

<a name="2.9"></a>
<a name="scene-structure"></a>
## 2.9 Scene Structure
Next to the project’s hierarchy, there’s also scene hierarchy. Use named empty game objects as scene folders.

<pre>
Debug
Management
UI
World
    Layers
    Objects
Gameplay
	Characters
	Items
  BattleSystem
_Dynamic
</pre>

 - All empty objects should be located at 0,0,0 with default rotation and scale.
 - For empty objects that are only containers for scripts, use “@” as prefix – e.g. @Cheats
 - When you’re instantiating an object in runtime, make sure to put it in `_Dynamic` – do not pollute the root of your hierarchy or you will find it difficult to 
 navigate through it.

**[⬆ Back to Top](#table-of-contents)**

<a name="scripts"></a>

## 3. Scripts

This section will focus on C# classes and their internals. When possible, style rules conform to Microsoft's C# standard.

### Sections
> 3.1 [Naming](#naming)

> 3.2 [Declarations](#declarations)

> 3.3 [Spacing](#spacing)

<a name="naming"></a>
## 3.1 Naming

On the whole, naming should follow C# standards.

### Namespaces

Namespaces are all **PascalCase**, multiple words concatenated together, without hyphens ( - ) or underscores ( \_ ). The exception to this rule are acronyms 
like GUI or HUD, which can be uppercase:

**AVOID**:

```csharp
itsdits.ravar.ui.dialog
```

**PREFER**:

```csharp
Itsdits.Ravar.UI.Dialog
```

### Classes & Interfaces

Classes and interfaces are written in **PascalCase**. For example `BattleMonster`. 

### Methods

Methods are written in **PascalCase**. For example `ExecuteTurn()`. 

### Fields

All non-static fields are written **camelCase**. Per Unity convention, this includes **public fields** as well.

For example:

```csharp
public class MyClass 
{
    public int publicField;
    int packagePrivate;
    private int myPrivate;
    protected int myProtected;
}
```

**AVOID:**

```csharp
private int _myPrivateVariable
```

**PREFER:**

```csharp
private int myPrivateVariable
```

Static fields are the exception and should be written in **PascalCase**:

```csharp
public static int TheAnswer = 42;
```
### Properties

All properties are written in **PascalCase**. For example:

```csharp
public int PageNumber 
{
    get { return pageNumber; }
    set { pageNumber = value; }
}
```

```csharp
public string Name => name;
```

### Parameters

Parameters are written in **camelCase**.

**AVOID:**

```csharp
void DoSomething(Vector3 Location)
```

**PREFER:**

```csharp
void DoSomething(Vector3 location)
```

Single character values are to be avoided except for temporary looping variables.

### Actions

Actions are written in **PascalCase**. For example:

```csharp
public event Action<int> ValueChanged;
```

### Misc

In code, acronyms should be treated as words. For example:

**AVOID:**

```csharp
XMLHTTPRequest
String URL
findPostByID
```  

**PREFER:**

```csharp
XmlHttpRequest
String url
findPostById
```

<a name="declarations"></a>
## Declarations

### Access Level Modifiers

Access level modifiers should be explicitly defined for classes, methods and member variables.

### Fields & Variables

Prefer single declaration per line.

**AVOID:**

```csharp
string username, twitterHandle;
```

**PREFER:**

```csharp
string username;
string twitterHandle;
```

### Classes

Exactly one class per source file, although inner classes are encouraged where scoping appropriate.

### Interfaces

All interfaces should be prefaced with the letter **I**. 

**AVOID:**

```csharp
RadialSlider
```

**PREFER:**

```csharp
IRadialSlider
```

<a name="spacing"></a>
## Spacing

Spacing is important, code should be easy to read and not visually offensive.

### Indentation

Indentation should be done using tabs.

#### Blocks

Indentation for blocks uses **4 spaces** for optimal readability:

**AVOID:**

```csharp
for (int i = 0; i < 10; i++) 
{
  Debug.Log("index=" + i);
}
```

**PREFER:**

```csharp
for (int i = 0; i < 10; i++) 
{
    Debug.Log("index=" + i);
}
```

#### Line Wraps

Indentation for line wraps should use **4 spaces** (not the default 8):

**AVOID:**

```csharp
CoolUiWidget widget =
        someIncrediblyLongExpression(that, reallyWouldNotFit, on, aSingle, line);
```

**PREFER:**

```csharp
CoolUiWidget widget =
    someIncrediblyLongExpression(that, reallyWouldNotFit, on, aSingle, line);
```

### Line Length

Lines should be no longer than **100** characters long.

### Vertical Spacing

There should be exactly one blank line between methods to aid in visual clarity 
and organization. Whitespace within methods should separate functionality, but 
having too many sections in a method often means you should refactor into
several methods.

`GameState` changes are especially important in the code, and therefore any calls to `state` should be separated with a blank line both above and below.

**AVOID:**

```csharp
CloseDialog();
state = GameState.World;
DoSomethingElse();
```

**PREFER:**

```csharp
CloseDialog();

state = GameState.World;

DoSomethingElse();
```

## Brace Style

All braces get their own line as it is a C# convention:

**AVOID:**

```csharp
class MyClass {
    void DoSomething() {
        if (someTest) {
          // ...
        } else {
          // ...
        }
    }
}
```

**PREFER:**

```csharp
class MyClass
{
    void DoSomething()
    {
        if (someTest)
        {
          // ...
        }
        else
        {
          // ...
        }
    }
}
```

Conditional statements are always required to be enclosed with braces,
irrespective of the number of lines required.

**AVOID:**

```csharp
if (someTest)
    doSomething();  

if (someTest) doSomethingElse();
```

**PREFER:**

```csharp
if (someTest) 
{
    DoSomething();
}  

if (someTest)
{
    DoSomethingElse();
}
```
## Switch Statements

Switch-statements come with `default` case by default (heh). If the `default` case is never reached, be sure to remove it.

**AVOID:**  
  
```csharp
switch (variable) 
{
    case 1:
        break;
    case 2:
        break;
    default:
        break;
}
```

**PREFER:**  
  
```csharp
switch (variable) 
{
    case 1:
        break;
    case 2:
        break;
}
```

## Language

Use US English spelling.

**AVOID:**

```csharp
string colour = "red";
```

**PREFER:**

```csharp
string color = "red";
```

The exception here is `MonoBehaviour` as that's what the class is actually called.

**[⬆ Back to Top](#table-of-contents)**

<a name="assets"></a>
<a name="4"></a>

## 4. Asset Naming Conventions
Naming conventions should be treated as law. A project that conforms to a naming convention is able to have its assets managed, searched, parsed, and 
maintained with incredible ease.

Most things are prefixed with the prefix generally being an acronym of the asset type followed by an underscore.

**Assets use [PascalCase](#cases)**

<a name="base-asset-name"></a>
<a name="4.1"></a>
### 4.1 Base Asset Name - `Prefix_BaseAssetName_Variant_Suffix`
All assets should have a _Base Asset Name_. A Base Asset Name represents a logical grouping of related assets. Any asset that is part of this logical group 
should follow the the standard of  `Prefix_BaseAssetName_Variant_Suffix`.

RavaR uses TexturePackerImporter to condense and process sprite sheets. All sprite sheets and tilesets should include a `.tpsheet` file with them.

##### Sprite Sheets

| Asset Type               | Asset Name   |
| ------------------------ | ------------ |
| Sprite Sheet             | SS_Gus       |
| TP Sheet                 | TP_Gus       |
| Battle Sprite            | BS_Gus       |

Individual sprites drop the prefix to shorten the name, but should be descriptive. For example: `Gus_WalkDown_2.png` or `Beam_Shoot_Yellow.png`.

##### Tilesets

| Asset Type               | Asset Name   |
| ------------------------ | ------------ |
| Terrain                  | TS_GrassLight|
| Objects                  | OS_HouseBlue |

Individual tiles drop the prefix to shorten the name, but should be descriptive. For example: `DirtPath_1_TopLeft.png` or `GrassPatch_2_TopRight.png`.

Tiles should be numbered and list the direction, starting with the top left and working right then downwards until the bottom right tile is reached. For example:

| Grass Tileset        |
| -------------------- |
| Grass_1_TopLeft      |
| Grass_2_TopRight     |
| Grass_3_BottomLeft   |
| Grass_4_BottomRight  |

**[⬆ Back to Top](#table-of-contents)**

<a name="conclusion"></a>
<a name="5"></a>

## 5. Conclusion

Congratulations on making it to the end of this long-winded guide. Hopefully the standards set here can be upheld during the development and maintenance of this
project, and saves us some valuable time and stress. Nobody is perfect, but we should strive to follow these guidelines as closely as possible to create a level of
consistency one would expect from a clock. 

Please feel free to make suggestions, corrections or improvements to this document as you feel necessary. If you find violations of these standards within the 
project, please raise an issue to bring it to our attention.

This guide was derived from a mixture of the following two style guides:

<a href="https://github.com/raywenderlich/c-sharp-style-guide">The Official raywenderlich.com C# Style Guide</a>

<a href="https://github.com/justinwasilenko/Unity-Style-Guide">justinwasilenko/Unity-Style-Guide</a>

