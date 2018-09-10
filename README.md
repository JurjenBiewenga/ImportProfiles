# ImportProfiles

ImportProfiles is a tool to automatically apply preset templates to your assets when they are imported.

# Basic usage
The profile editor is found under `Window/Import Profiles Editor`, that should open up this window.
![Editor](https://github.com/JurjenBiewenga/ImportProfiles/blob/master/img/Editor.png?raw=true)
This is the basic profile editor view, from here you can edit your current profiles and create new profiles.

Creating a new profile is done using the `Create` button, this will allow you to select what kind of profile you would like to make.
![Create](https://github.com/JurjenBiewenga/ImportProfiles/blob/master/img/Editor_Create.png?raw=true)

After you've created your template, you can edit your template here. This looks almost exactly like the default importer for the profile type, these values get applied to the asset that will be impoted.
![Template](https://github.com/JurjenBiewenga/ImportProfiles/blob/master/img/Editor_Template.png?raw=true)

Last but not least, the Wildcard query field will allow you to automatically apply profiles to your assets, this wildcard query is compared to the asset path on import. If the wildcard has a valid match the profile is applied.
![Query](https://github.com/JurjenBiewenga/ImportProfiles/blob/master/img/Editor_Query.png)
