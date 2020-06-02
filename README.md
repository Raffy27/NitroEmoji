# NitroEmoji
<img src="https://i.imgur.com/07PFZqy.png"> NitroEmoji is a small application that allows Discord users without an active Nitro subscription to automatically download and use custom emojis from servers that they are members of. It also supports cloning custom emojis, given the appropriate emoji ID.

## Features
* Login with session token or email/password
* Request available servers
* Request available emojis
* Download and resize static and animated emojis
* Add custom emojis
* Use saved emojis in other Discord conversations

**Currently, saved emojis appear distorted on mobile versions of Discord.** This is due to Discord's platform-dependent handling of low resolution images.

## Screenshots
<details>
    <summary>Show screenshots</summary>
    <img alt="Login screen" src="https://i.imgur.com/eR2uhAv.png">
    <img alt="Emoji selection" src="https://i.imgur.com/QoUE2C6.png">
    <img alt="Custom emojis" src="https://i.imgur.com/BIaAqia.png">
</details>

## How to use
1. Login with email or session token
    * Copy session token and press **Ctrl+T**
2. Wait for your emojis to load
3. Add custom emojis (optional)
    * Copy emoji ID and press **Ctrl+N**
4. Drag and Drop an emoji onto an open Discord conversation to use it

Press **F1** for about/help.

## Getting started
This section covers the recommended software and dependencies needed to compile and debug the project.

### Prerequisites
Dev environment:
* Microsoft Visual Studio 2019
* .NET Framework 4.7.2

### Dependencies
* **FluentWPF** for the Acrylic Window style
* **LoadingIndicators.WPF** for the preloader
* **Json.NET** for parsing request data
* **WpfAnimatedGif** for displaying animated emojis
* **Gifsicle** for resizing animated emojis

## Releases
For active releases and pre-compiled binaries, see <a href="https://github.com/Raffy27/NitroEmoji/releases" target="_blank">Releases</a>.

## License
This project is licensed under the MIT License -  see the <a href="https://github.com/Raffy27/NitroEmoji/blob/master/LICENSE" target="_blank">LICENSE</a> file for details. For the dependencies, all rights belong to their respective owners. These should be used according to their respective licenses.![][image_ref_mt9roddj]
