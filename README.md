# NoJoy

Simple tool for enabling/disabling game controllers quickly.

<img alt="screenshot" src="https://github.com/ssg/NoJoy/assets/241217/2ef30a9f-77a4-4412-b763-d38218a5d1c3" width=500>

# Release Notes

## v0.3.1 (2023-07-23)
(Minor release) Fix version string. This release is otherwise identical to 0.3.0.
0.3.0 will be removed from release lists.

## v0.3.0 (2023-07-20)
- Faster enable/disable and fix some incompatibilities with certain Windows versions.
- More intuitive power icon instead of confusing enable/disable labels.
- This version requires 64-bit version of Windows due to changes.

## v0.2.3 (2023-07-11)
- Fix crash when PnP operation generates no output
- Read version from the executable instead of having it hardcoded.

## v0.2.2 (2018-08-22)
- Fix for Saitek X52 et al

## v0.2.1 (2018-08-14)
- Fix the incorrect appearance of "No game controllers found" message
- Fix the display of error messages

## v0.2 (2018-08-13)
- Error messages - thanks to Uygar Yilmaz ([@uygary](https://github.com/uygary))
- When no game controller is found the error is properly displayed

## v0.1 (2018-08-12)
- Initial release

# History

My friend [@detaybey](https://github.com/detaybey) gifted me the game [INSIDE](http://www.playdead.com/games/inside/) 
a couple of years ago. I started up the game and noticed that the guy was always running without me 
doing anything. I thought it was designed like that, went along with it, and died soon. Then I noticed it 
was getting analog input signals from my HOTAS joystick and registering them as moves, even though I didn't 
select it as an input device.

I wished then there were a tool that allowed me to disable and enable my HOTAS setup quickly. I could
unplug the joystick or you know, disable it from Device Manager but they all felt cumbersome. So I went
for the second best option and stopped playing INSIDE.

Yesterday, I fell asleep during the day and was wide awake at night. So I gave it a chance and developed
this tool. I was looking forward on making something on WPF as I'm quite fond of it. I liked the development
process a lot. So, essentially this is my first WPF app.

# Technical Notes

I enumerate gaming devices using WMI. I didn't want to enter the world of SetupDi to enable/disable so the tool
simply invokes PowerShell to do that, which might be Windows 10 specific. If you have the will, feel free 
to adapt it to a native method.

# To Do's

- More test coverage
- Support for Unity vJoy devices
- Better UX for the discrepancy between green light and "Disable" verb
- Native enable/disable for better compatibility with older Windows versions
- Proper setup and signed binaries, so UAC doesn't perplex the user

# Contribution

Please test it on different joysticks as I'm not sure how it will perform. Saitek setup is basically a hack
and other brands might need different hacks as well. Unfortunately Windows doesn't provide an easy path to 
access correct identifiers of gaming devices, or even better a simpler API to disable/enable input devices.

Feel free to send in pull requests aligned with TODO as well.

# License

This software is licensed with Apache License v2.0.
