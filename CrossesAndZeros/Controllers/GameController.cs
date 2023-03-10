using CrossesAndZeros.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace CrossesAndZeros.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        /// MoveCode: (also represents a player sending the request) a.k.a security validation going on cliet side
        /// 0 - nothing
        /// 1 - cross
        /// 2 - zero
        /// 
        /// 
        private readonly CrossesAndZerosContext db;
        public GameController(CrossesAndZerosContext db)
        {
            this.db = db;
        }
        [HttpPost]
        public int CreateGame([FromHeader] string Description)
        {
            Game game = new Game()
            {
                Description = Description,
                SquareStates = "000000000"
            };
            db.Games.Add(game);
            db.SaveChanges();
            return game.Id;
        }
        [HttpGet("{GameId}")]
        public string GetStates([FromRoute] int GameId)
        {
            return (from p in db.Games where p.Id==GameId select p.SquareStates).First();
        }
        
        /// <summary>
        /// makes move from player sending the request
        /// </summary>
        /// <param name="GameId"></param>
        /// <param name="MoveCode"></param>
        /// <param name="SquarePosition"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpPost]
        public string MakeMove([FromHeader] int GameId, [FromHeader] int MoveCode, [FromHeader] int SquarePosition)
        {
            //TODO: generate server response with Code 422
            if(!(MoveCode==1||MoveCode==2))
            {
                throw new NotImplementedException("MoveCode is 1 for cross and 2 for zero!");
            }
            if (SquarePosition<1||SquarePosition>9)
            {
                throw new NotImplementedException("SquarePosition should be in the range of [1-9]");
            }
            Game game = (from p in db.Games where p.Id==GameId select p).First();
            var g = game.SquareStates.ToCharArray();

            g[SquarePosition - 1] = MoveCode.ToString().ToCharArray()[0];
            game.SquareStates = new string(g);

            db.Games.Update(game);
            db.SaveChanges();
            return game.SquareStates;
        }
        /// <summary>
        /// Call after every move before MakeMove()
        /// Checks if player sending the request is won
        /// </summary>
        /// <param name="GameId"></param>
        /// <param name="MoveCode"></param>
        /// <returns></returns>
        [HttpGet]
        public bool AmIWon([FromHeader] int GameId, [FromHeader] int MoveCode)
        {
            string states = (from p in db.Games where p.Id == GameId select p.SquareStates).First();
            Console.WriteLine(states);
            int[] temp = new int[9];
            for(int i=0; i<9; i++)
            {
                var charg = new char[] { states[i] };
                temp[i] = Convert.ToInt32(new string(charg));
                Console.WriteLine(temp[i]);
            }
            
            int[,] gs = new int[3, 3]
            {
                {temp[0],temp[1],temp[2] },
                {temp[3],temp[4],temp[5] },
                {temp[6],temp[7],temp[8] }
            };
            bool IsWinningSequence = true;
            //horizontal
            for(int i=0; i < 3; i++)
            {
                for(int j = 0; j < 3; j++)
                {
                    if (IsWinningSequence)
                    {
                        if(gs[i, j] != MoveCode)
                        {
                            IsWinningSequence= false;
                            break;
                        }
                    }
                }
                if (IsWinningSequence)
                {
                    return true;
                }
                IsWinningSequence = true;
            }
            IsWinningSequence= true;
            //vertical
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (IsWinningSequence)
                    {
                        if (gs[j, i] != MoveCode)
                        {
                            IsWinningSequence = false;
                            break;
                        }
                    }

                }
                if (IsWinningSequence)
                {
                    return true;
                }
                IsWinningSequence = true;
            }

            //diagonal (manualy)
            if (
                gs[0, 0] == MoveCode &&
                gs[1, 1] == MoveCode &&
                gs[2, 2] == MoveCode ||
                gs[0, 2] == MoveCode &&
                gs[1, 1] == MoveCode &&
                gs[2, 0] == MoveCode 
                )
            {
                return true;
            }
            return false;
        }
    }
}
