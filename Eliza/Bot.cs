
using System;
using System.Collections.Generic;

using System.Text;
using System.Threading;
using System.Net;

namespace Eliza
{
  public class Bot
  {
    protected ElizaApi eliza;
    private volatile bool _shouldStop = false;

    public Bot(ElizaApi eliza)
    {
      this.eliza = eliza;
    }

    protected virtual void AcceptInvite(Game game)
    {
      eliza.AcceptInvite(game.Id);
    }
    protected virtual bool IdleGame(Game game)
    {
      return true;
    }

    protected virtual void ProcessGame(Game detailed)
    {
    }

    public void DoWork()
    {
      while (!_shouldStop)
      {
        try
        {
          List<Game> games = eliza.GetGamesFromHeadquarters();
          foreach (Game game in games)
          {
            if (game.RequiresAnInviteAccept)
            {
              AcceptInvite(game);
            }
            else if (!game.IsInNeedOfAttention)
            {
              if (IdleGame(game))
                continue;
            }
            ProcessGame(eliza.GetGameState(game.Id));
          }
          Thread.Sleep(1000);
        }
        catch (WebException wex)
        {
          Console.WriteLine(wex);
          _shouldStop = true;
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex);
        }
      }
    }

    public void RequestStop()
    {
      _shouldStop = true;
    }

  }
}
