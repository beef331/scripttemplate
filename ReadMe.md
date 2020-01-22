# Script Template
An easy to script templator, aimed to make it so restarting is not required to add templates to unity.

## How To Use
Simply add the package using the Unity Package Manager.

#### For 2019.3+

Open the UPM, click the plus in the top left, then click add from git url, finally paste in 
`https://github.com/beef331/scripttemplate.git`
#### For 2019.2 and below
Open `./Packages/manifest.json` then add under dependencies 
`"com.beef331.scripttemplate" : "https://github.com/beef331/scripttemplate.git"`

After installing you have two methods to make a script a template:

```
A)

[ScriptTemplate.Templated]
public class Script{

}

B)

using ScriptTemplate;

[Templated]
public class Script{

}
```
Then on recompile the appropriate files will be made to enable the templated files.

