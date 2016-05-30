#safetyzone-api

##Intro

This repository encompasses the Web API portion of the SafetyZone project, an application I completed in a group for university in 2015. It runs on an Azure App Service instance, so it's pretty cheap to deploy. 

It's not really ready to set-and-forget with an Azure App Service instance, as it requires an associated database schema that aligns with the Bureau of Statistics' shapefile, and inclusion of LGA crime statistics, although there is an API endpoint for that.
I recommend reading the API doc for starters, and give me a ping @busbymitch if you want to get involved.


##What is safetyzone?

From the submission docs: "Our product, SafetyZone, is a safety app aimed to help people feel safer when walking alone, especially at night."

Basically, it gets the user's location (longitude and latitude) and submits it to the API we built; this API works out the local government area (NSW only) the user is in, and then gets the associated assault statistics for the local government area. The app also has a bunch of safety tips, and allows the user to set a designated contact to notify if they enter an unsafe local government area. We initially planned to use more specific and granular statistics, but BOCSAR denied our requests for more in depth datasets than at the LGA level. :(

##Companion app source

[Linky](https://github.com/mitchellbusby/safetyzone-app)

