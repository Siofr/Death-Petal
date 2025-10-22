# Death Petal
Repository for Death Petal. This readme contains information related to Unity folder structure, folder naming conventions and code conventions.

## Unity Project Structure
This section containts information on accepted folder and file naming conventions and structure, these conventions should be followed by everyone to ensure that the project folders remain easy to navigate for all developers. It is important that our folders and files both have strong structure and naming conventions as it should be possible to navigate and find what we need both through using the dropdown and the search function within Unity.

### Folder and File Naming Conventions
When naming files and folders we should use prefixes and suffixes to make them more easily searchable. If a prefix or suffix does not already exist for a specific type of file in the project then a unique one should be created and added to this list, this list is non exhaustive and should be added to when necessary. Names should be short and consistent.

<ins>General Naming structure example</ins>

[prefix]\_[type of file]\_[name]\_[number]

<ins>Prefixes</ins>

- pl - All files associated with the player character
- en - All files associated with enemy characters
- npc - All files associated with non-player characters

<ins>Type of File</ins>

- Snd - Audio and Sound Files
- Tex - Texture Files
- Mat - Material Files

To be expanded

### Folder Structure
Below is an example of the intended Unity folder structure developers to follow
- Code
  - Scripts
    - Within these folders we should seperate scripts by the entity they are attached to, and further by Monobheaviour and Scriptable Objects
  - Shaders
- Art
  - Within this folder art should be broken into seperate folders based on their entity such as Player, Enemies or Environment for example
- Audio
  - FMOD Audio Assets
    - Platform
- Docs
  - Folder for concept art related to the project or important documentation
- Level
  - Scenes
    - This folder should be further broken down based on the Level and their specific components such as NavMeshData or Postprocessing
  - Prefabs
- Plugins
- Editor
