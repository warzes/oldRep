using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Entity
{
	public long EntityId;
	public string Name;
	public int RegionId;
	public int X;
	public int Z;
	public byte Direction;

	public Transform Transform;
}

public class Creature : Entity
{
}