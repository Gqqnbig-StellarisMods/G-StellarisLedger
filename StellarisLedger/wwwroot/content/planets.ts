function findTileTd(id: number)
{
	let r = Math.floor(id / 5);
	let c = id % 5;
	return this.closest(".planet").find(`table.tiles  tr:nth-child(${r + 1}) td:nth-child(${c + 1})`);
}


function isTileAdjacent(id1: number, id2: number) :boolean
{
	return id1 % 5 !== 0 && id1 - 1 === id2 ||
		   id1 % 5 !== 4 && id1 + 1 === id2 ||
		   Math.floor(id1 / 5) > 0 && id1 - 5 === id2 ||
		   Math.floor(id1 / 5) < 5 && id1 + 5 === id2;
}