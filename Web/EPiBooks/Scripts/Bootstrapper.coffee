﻿class Bootstrapper
	setup: ->
		bodyId = $("body").attr 'id'
		if bodyId is "Startpage"
			startPage = new Startpage
			startPage.load()
			return
		
($ document).ready ->
	strapper = new Bootstrapper
	strapper.setup()