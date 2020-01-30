"use strict";
let hexagonRadius = 5;
let elementsPerRow = 100;

function createElements(elements)
{
	//console.log(elements.target.response);

	// Assuming you get an array of objects.
	elements = JSON.parse(elements.target.response);
	let infoDiv = document.getElementById("infos");
	infoDiv.innerHTML = "";
	let numberOfRows = elements.vertexPositions.length % elementsPerRow;
	let numberOfColumns = elements.vertexPositions.length / numberOfRows;
	let heatmapHeight = (numberOfRows + 1/3) * 3/2 * hexagonRadius;
	let heatmapWidth = (numberOfColumns + 1/2) * Math.sqrt(3) * hexagonRadius;
	let margin = {
		top: 50,
		right: 20,
		bottom: 20,
		left: 50
	}

	let positions = [];
	let temperatures = [];

	for (let i = 0; i < elements.temperatures.length; i++)
	{
		let div = document.createElement("div");
		div.id = i;
		div.innerHTML = elements.temperatures[i];
		infoDiv.appendChild(div);
		//d3.hexbin(vertex)

		//positions[i] = [(elements.vertexPositions[i].x) * 100, (elements.vertexPositions[i].y) * 100];
		positions[i] = [i % elementsPerRow * hexagonRadius + hexagonRadius, Math.round(i / elementsPerRow) * hexagonRadius + hexagonRadius];
		temperatures[i] = "rgb( 255, " + Math.round(elements.temperatures[i] * 255) + ", 0)";
	}

	document.getElementById("graph").innerHTML = "";
	let svg = d3.select("#graph")
				.append("svg")
				.attr("width", heatmapHeight)
				.attr("height", heatmapWidth)
				.append("g");
	
	let hexbin = d3.hexbin().radius(hexagonRadius);
	
	let customData = hexbin(positions);
	for (let i = 0; i < customData.length; i++)
	{
		customData[i].color = temperatures[i];
	}

	svg.append("g")
		.selectAll(".hexagon")
		.data(customData)
		.enter().append("path")
		.attr("class", "hexagon")
		.attr("d", function(d)
		{
			return "M" + d.x + "," + d.y + hexbin.hexagon();
		})
		.attr("stroke", "white")
		.attr("stroke-width", "1px")
		.style("fill", function(d)
		{
			return d.color;
		});

	/*
	elements.forEach(funciton (element)
	{
		let div = document.createElement("div");
		div.id = element.
		let div = document.createElements(element.temperatures);
		div.innerHTML = element.text;
	});
	*/
}

function reloadData()
{
	setTimeout(
		function ()
		{
			let request = new XMLHttpRequest();
			request.onload = createElements;
			request.open("GET", "Data/jsontest.json", true);
			request.send();
			console.log("noice");
			reloadData();
		}, 1000);
}

reloadData();