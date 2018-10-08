using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Network;
using Core.Const;
using Server.Network.Sending;

namespace Server.World.Entities
{
	public abstract class Creature : Entity, IDisposable
	{
		// General
		// ------------------------------------------------------------------
		public Client Client { get; set; }
		public string Name { get; set; }
		public override int RegionId { get; set; }
		public Locks Locks { get; protected set; }


		// Movement
		// ------------------------------------------------------------------
		private Position _position, _destination;
		private DateTime _moveStartTime;
		private double _moveDuration, _movementX, _movementY;

		public byte Direction { get; set; }
		public bool IsMoving { get { return (_position != _destination); } }
		public bool IsWalking { get; protected set; }
		public double MoveDuration { get { return _moveDuration; } }

		public Location WarpLocation { get; set; }
		public Location LastLocation { get; set; }
		public Location FallbackLocation { get; set; }
		public Location DungeonSaveLocation { get; set; }

		/// <summary>
		/// True while character is warping somewhere.
		/// </summary>
		public bool Warping { get; set; }

		// Combat
		// ------------------------------------------------------------------

		// Stats
		// ------------------------------------------------------------------


		protected Creature()
		{

		}

		public virtual void LoadDefault()
		{
		}

		public virtual void Dispose()
		{
		}

		public override Position GetPosition()
		{
			if (!this.IsMoving)
				return _position;

			var passed = (DateTime.Now - _moveStartTime).TotalSeconds;
			if (passed >= _moveDuration)
				return this.SetPosition(_destination.X, _destination.Y);

			var xt = _position.X + (_movementX * passed);
			var yt = _position.Y + (_movementY * passed);

			return new Position((int)xt, (int)yt);
		}
		
		public Position SetPosition(int x, int y)
		{
			return _position = _destination = new Position(x, y);
		}
		
		public void SetLocation(int region, int x, int y)
		{
			this.RegionId = region;
			this.SetPosition(x, y);
		}

		public void SetLocation(Location loc)
		{
			this.SetLocation(loc.RegionId, loc.X, loc.Y);
		}

		public abstract bool Warp(int regionId, int x, int y);

		public bool Warp(Location loc)
		{
			return this.Warp(loc.RegionId, loc.X, loc.Y);
		}

		public bool Warp(int regionId, Position position)
		{
			return this.Warp(regionId, position.X, position.Y);
		}

		public Locks Lock(Locks locks, bool updateClient = false)
		{
			var prev = this.Locks;
			this.Locks |= locks;

			if (updateClient)
				Send.CharacterLock(this, locks);

			return this.Locks;
		}
	}
}
