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

function flatPaths(nodes: SearchNode[]): SearchNode[][]
{
	if (!nodes)
		return [];

	let list: SearchNode[][] = [];
	for (let i = 0; i < nodes.length; i++)
	{
		if (nodes[i].children && nodes[i].children.length > 0)
		{
			let flat = flatPaths(nodes[i].children);
			for (let j = 0; j < flat.length; j++)
				flat[j].unshift(nodes[i]);
			list = list.concat(flat);
		} else
		{
			list.push([nodes[i]]);
		}
	}
	return list;
}

function joinStringWithSeparator(s1: string, separator: string, s2: string): string
{
	if (s1 && s2)
		return s1 + separator + s2;
	if (s1)
		return s1;
	if (s2)
		return s2;
	return "";
}

function joinString(s1: string, s2: string): string
{
	if (s1 && s2)
		return s1 + s2;
	if (s1)
		return s1;
	if (s2)
		return s2;
	return "";
}

/**
 * row[0]是首都节点，后面可能还会有首都节点，把这两个节点合并起来。
 * @param row
 */
function mergeCapitalNodes(row: SearchNode[]): void
{
	for (let i = 1; i < row.length; i++)
	{
		if (row[i].tileId === row[0].tileId)
		{
			if (row[i].description)
				row[0].description = joinStringWithSeparator(row[0].description, "，", row[i].description);
			row[0].bonus += row[i].bonus;
			row.splice(i, 1);
			break;
		}
	}
}



class SearchNode
{
	tileId: number;
	bonus: number = 0;
	description: string = "";

	children: SearchNode[];

	constructor(tileId: number)
	{
		this.tileId = tileId;
	}


	toString()
	{
		if (this.tileId >= 0)
		{
			if (this.description)
				return `<span class="tileId">${this.tileId}</span>：${this.description}`;
			else
				return "";
		} else
			return this.description;
	}
}

