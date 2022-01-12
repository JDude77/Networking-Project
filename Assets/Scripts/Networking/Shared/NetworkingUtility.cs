namespace NetworkingUtility
{
    using System.Linq;
    using System;

    static class NetUtil
    {
        public static byte[] Serialise(Packet packet)
        {
            var array = BitConverter.GetBytes(packet.id);

            return array.ToArray();
        }

        public static byte[] Serialise(PlayerData packet)
        {
            var array = BitConverter.GetBytes(packet.type)
                .Concat(BitConverter.GetBytes(packet.id))
                .Concat(BitConverter.GetBytes(packet.yPosition))
                .Concat(BitConverter.GetBytes(packet.yVelocity))
                .Concat(BitConverter.GetBytes(packet.gravityDirection))
                .Concat(BitConverter.GetBytes(packet.faceShowing))
                .Concat(BitConverter.GetBytes(packet.colliding))
                .Concat(BitConverter.GetBytes(packet.dead));

            return array.ToArray();
        }

        public static byte[] Serialise(PlatformData packet)
        {
            var array = BitConverter.GetBytes(packet.type)
                .Concat(BitConverter.GetBytes(packet.id))
                .Concat(BitConverter.GetBytes(packet.platformID))
                .Concat(BitConverter.GetBytes(packet.yPosition))
                .Concat(BitConverter.GetBytes(packet.xPosition))
                .Concat(BitConverter.GetBytes(packet.xVelocity))
                .Concat(BitConverter.GetBytes(packet.xScale))
                .Concat(BitConverter.GetBytes((int)packet.colour))
                .Concat(BitConverter.GetBytes(packet.destroyed));

            return array.ToArray();
        }

        public static void Deserialise(byte[] data, out Packet packet)
        {
            int index = 0;

            packet = new Packet();

            packet.type = BitConverter.ToInt16(data, index);
            index += sizeof(short);
            packet.id = BitConverter.ToInt32(data, index);
        }

        public static void Deserialise(byte[] data, out PlayerData packet)
        {
            int index = 0;

            packet = new PlayerData();

            //Type
            packet.type = BitConverter.ToInt16(data, index);
            index += sizeof(short);
            //ID
            packet.id = BitConverter.ToInt32(data, index);
            index += sizeof(int);
            //Y Pos
            packet.yPosition = BitConverter.ToSingle(data, index);
            index += sizeof(float);
            //Y Vel
            packet.yVelocity = BitConverter.ToSingle(data, index);
            index += sizeof(float);
            //Gravity Direction
            packet.gravityDirection = BitConverter.ToInt32(data, index);
            index += sizeof(int);
            //Face Shown
            packet.faceShowing = BitConverter.ToInt32(data, index);
            index += sizeof(int);
            //Is Colliding
            packet.colliding = BitConverter.ToBoolean(data, index);
            index += sizeof(bool);
            //Is Dead
            packet.dead = BitConverter.ToBoolean(data, index);
        }

        public static void Deserialise(byte[] data, out PlatformData packet)
        {
            int index = 0;

            packet = new PlatformData();

            //Type
            packet.type = BitConverter.ToInt16(data, index);
            index += sizeof(short);
            //ID
            packet.id = BitConverter.ToInt32(data, index);
            index += sizeof(int);
            //Platform ID
            packet.platformID = BitConverter.ToInt32(data, index);
            index += sizeof(int);
            //Y Pos
            packet.yPosition = BitConverter.ToSingle(data, index);
            index += sizeof(float);
            //X Pos
            packet.xPosition = BitConverter.ToSingle(data, index);
            index += sizeof(float);
            //X Vel
            packet.xVelocity = BitConverter.ToSingle(data, index);
            index += sizeof(float);
            //X Scale
            packet.xScale = BitConverter.ToSingle(data, index);
            index += sizeof(int);
            //Colour
            packet.colour = (PlatformScript.COLOUR)BitConverter.ToInt32(data, index);
            index += sizeof(int);
            //Is Destroyed
            packet.destroyed = BitConverter.ToBoolean(data, index);
        }
    }

    public class Packet
    {
        public short type;
        public int id;
        private static int idGen = 0;

        public Packet()
        {
            type = 0;

            id = idGen;

            idGen++;
        }
    }

    public class PlayerData : Packet
    {
        public float yPosition;
        public float yVelocity;
        public int gravityDirection;
        public bool colliding;
        public bool dead;
        public int faceShowing;

        public PlayerData() : base()
        {
            type = 1;
            yPosition = 0.0f;
            yVelocity = 0.0f;
            gravityDirection = 0;
            colliding = false;
            dead = false;
            faceShowing = 0;
        }
    }

    public class PlatformData : Packet
    {
        public int platformID;
        public float yPosition;
        public float xPosition;
        public float xVelocity;
        public float xScale;
        public PlatformScript.COLOUR colour;
        public bool destroyed;

        public PlatformData() : base()
        {
            type = 2;
            platformID = -1;
            yPosition = 0.0f;
            xPosition = 0.0f;
            xVelocity = 0.0f;
            xScale = 0.0f;
            colour = PlatformScript.COLOUR.YELLOW;
            destroyed = false;
        }
    }
}