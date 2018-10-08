using Core.Const;
using System;

namespace Login.Database
{
	public class Character
	{
		public long EntityId { get; set; }
		public long CreatureId { get; set; }
		public string Name { get; set; }
		public string Server { get; set; }

		public int Region { get; set; }
		public int X { get; set; }
		public int Y { get; set; }

		public short AP { get; set; }
		public float Life { get; set; }
		public float Mana { get; set; }
		public float Stamina { get; set; }
		public float Str { get; set; }
		public float Int { get; set; }
		public float Dex { get; set; }
		public float Will { get; set; }
		public float Luck { get; set; }
		public short Defense { get; set; }
		public float Protection { get; set; }

		/// <summary>
		/// Time at which the character may be deleted for good.
		/// </summary>
		/// <remarks>
		/// If MinValue, the character is normal.
		/// If MaxValue, it's "gone".
		/// If it's above Now the character can be recovered.
		/// If it's below Now, the character can be deleted.
		/// </remarks>
		public DateTime DeletionTime { get; set; }

		/// <summary>
		/// Deletion state of the character, based on DeletionTime.
		/// 0: Normal, 1: Recoverable, 2: DeleteReady, 3: ToBeDeleted
		/// </summary>
		public DeletionFlag DeletionFlag
		{
			get
			{
				if (this.DeletionTime == DateTime.MaxValue)
					return DeletionFlag.Delete;
				else if (this.DeletionTime <= DateTime.MinValue)
					return DeletionFlag.Normal;
				else if (this.DeletionTime >= DateTime.Now)
					return DeletionFlag.Recover;
				else
					return DeletionFlag.Ready;
			}
		}

		public Character()
		{
			this.Life = 10;
			this.Mana = 10;
			this.Stamina = 100;
			this.Str = 10;
			this.Int = 10;
			this.Dex = 10;
			this.Will = 10;
			this.Luck = 10;
		}
	}
}
