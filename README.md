# NoJoy

Simple tool for enabling/disabling game controllers quickly.

![screenshot](https://github.com/ssg/NoJoy/blob/master/screenshot.png?raw=true)

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

# Contribution

Please test it on different joysticks as I'm not sure how it will perform. Saitek setup is basically a hack
and other brands might need different hacks as well. Unfortunately Windows doesn't provide an easy path to 
access correct identifiers of gaming devices, or even better a simpler API to disable/enable input devices.

# License

This software is licensed with Apache License v2.0.