using System.Collections.Generic;
using System.Drawing;

public class Hand : List<Card>
{
    private Game game;

    public Hand(Game g) : base()
    {
        game = g;
    }
        public void ShiftCards()
    {
        for (int i = 0; i < Count; i++)
        this[i].Location =
        this[i].prevLocation = 
        new Point(Card.cardWidth * i, game.form.workingHeight - Card.cardHeight);
    }
}
