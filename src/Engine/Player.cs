using System;

namespace Engine
{
    public class Player
    {
        public string Name { get; set; }
        public int Peice1Position { get; set; }
        public int Peice2Position { get; set; }
        public int Peice3Position { get; set; }
        public int Peice4Position { get; set; }
        
        public int RoleDice()

        {
            Random random = new Random();
            return random.Next(1, 7);            
        }        
    }   

}
