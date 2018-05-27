using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STVRogue.Utils;

namespace STVRogue.GameLogic
{
    public class Item
    {
        public String id; //id of the item
        public Boolean used = false; //if the item is used it is true
        public Item() { }
        public Item(String id) { this.id = id; }

        virtual public void use(Player player)
        {
            if (used) //if the item is used it rejects
            {
                Logger.log("" + player.id + " is trying to use an expired item: "
                              + this.GetType().Name + " " + id
                              + ". Rejected.");
                return;
            }
            //otherwise player uses the item
            Logger.log("" + player.id + " uses " + this.GetType().Name + " " + id);
            used = true;
			player.bag.Remove(this); //added this on 26th of May, because implementation assumes that when player
                                    //uses an item, item get removed from the bag
        }
    }

    public class HealingPotion : Item
    {
        public uint HPvalue; //hp value of the healing potion

        /* Create a healing potion with random HP-value */
        public HealingPotion(String id)
            : base(id)
        {
            HPvalue = (uint)RandomGenerator.rnd.Next(10) + 1; //hp value is between 1-10
        }

        //overrides item use, increases player's hp when it is used
        override public void use(Player player)
        {
            base.use(player);
            player.HP = (int)Math.Min(player.HPbase, player.HP + HPvalue);//player's hp can not exceed HPbase
        }
    }

    public class Crystal : Item
    {
        public Crystal(String id) : base(id) { } //calls Item's constructor
        //overrides item's use, makes player accelerated and disconnects the dungeon when it is used on a bridge
        override public void use(Player player)
        {
            base.use(player);
            player.accelerated = true; //if player uses magic crystal, player gets accelerated
			if (player.location is Bridge) //if player is at the bridge
				player.dungeon.disconnect(player.location as Bridge); //dungeon gets disconnected
        }
    }
}
