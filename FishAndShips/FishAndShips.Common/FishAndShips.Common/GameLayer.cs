using System;
using System.Collections.Generic;
using CocosSharp;
using Microsoft.Xna.Framework;

namespace FishAndShips.Common
{
    public class GameLayer : CCLayerColor
    {
        CCSprite fishSprite;
        CCSprite netSprite;
        CCSprite shipSprite;
        CCLabel scoreLabel;
        long gameStartTimeMillisecounds;
        float fishY = Constants.Height / 2;
        CCDrawNode drawNode;
        bool gameStarted = false;

        int score;

        public GameLayer() : base(CCColor4B.Black)
        {
            Init();            
        }

        public void Init()
        {
            // "paddle" refers to the paddle.png image
            fishSprite = new CCSprite("fish");
            fishSprite.PositionX = 70;
            fishSprite.PositionY = fishY;
            fishSprite.ContentSize = new CCSize(120, 60);
            AddChild(fishSprite);

            netSprite = new CCSprite("net");
            netSprite.PositionX = Constants.Width;
            netSprite.PositionY = Constants.Height - 235;
            netSprite.ContentSize = new CCSize(300, 470);
            AddChild(netSprite);

            shipSprite = new CCSprite("ship");
            shipSprite.PositionX = Constants.Width;
            shipSprite.PositionY = 235;
            shipSprite.ContentSize = new CCSize(256, 76);
            AddChild(shipSprite);

            scoreLabel = new CCLabel("Score: 0", "Arial", 50, CCLabelFormat.SystemFont);
            scoreLabel.PositionX = Constants.Width - 200;
            scoreLabel.PositionY = Constants.Height;
            scoreLabel.AnchorPoint = CCPoint.AnchorUpperLeft;
            AddChild(scoreLabel);

            drawNode = new CCDrawNode();
            AddChild(drawNode);
        }

        void RunGameLogic(float frameTimeInSeconds)
        {
            // New Code:
            // Check if the two CCSprites overlap...
            bool collision = netSprite.BoundingBoxTransformedToParent.IntersectsRect(
                fishSprite.BoundingBoxTransformedToParent);

            bool collision2 = shipSprite.BoundingBoxTransformedToParent.IntersectsRect(
                fishSprite.BoundingBoxTransformedToParent);

            if (collision || collision2)
            {
                scoreLabel.Text = "Game over! Score: " + score + "\nTap to restart!";
                scoreLabel.SystemFontSize = 100;
                scoreLabel.PositionX = Constants.Width / 2;
                scoreLabel.PositionY = Constants.Height / 2;
                scoreLabel.AnchorPoint = CCPoint.AnchorMiddle;
                gameStarted = false;         
            }
            else
            {
                // First let’s get the ball position:   
                float ballRight = netSprite.BoundingBoxTransformedToParent.MaxX;
                float ballLeft = netSprite.BoundingBoxTransformedToParent.MinX;
                // Then let’s get the screen edges
                float screenRight = VisibleBoundsWorldspace.MaxX;
                float screenLeft = VisibleBoundsWorldspace.MinX;

                long millisecounds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                netSprite.PositionX = Constants.Width - (((millisecounds - gameStartTimeMillisecounds) / 2) % Constants.Width);
                shipSprite.PositionX = Constants.Width - (((millisecounds - gameStartTimeMillisecounds) / 10) % Constants.Width);

                drawNode.Clear();
                drawNode.DrawLine(new CCPoint(0, fishY), new CCPoint(Constants.Width, fishY), new CCColor4B(100, 100, 100));

                fishSprite.PositionY = (fishY + (int)(Math.Sin(millisecounds / 400d) * 200));

                score = (int)((millisecounds - gameStartTimeMillisecounds) / 1000);
                scoreLabel.Text = "Score: " + score;
            }
        }

        protected override void AddedToScene()
        {
            base.AddedToScene();

            // Use the bounds to layout the positioning of our drawable assets
            CCRect bounds = VisibleBoundsWorldspace;

            // Register for touch events
            var touchListener = new CCEventListenerTouchAllAtOnce();
            touchListener.OnTouchesEnded = OnTouchesEnded;
            touchListener.OnTouchesMoved = HandleTouchesMoved;
            
            AddEventListener(touchListener, this);
        }

        void OnTouchesEnded(List<CCTouch> touches, CCEvent touchEvent)
        {
            if (touches.Count > 0)
            {
                if(!gameStarted)
                {
                    RemoveAllChildren();
                    Init();
                    gameStartTimeMillisecounds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                    Schedule(RunGameLogic);
                    gameStarted = true;
                }
            }
        }

        void HandleTouchesMoved(System.Collections.Generic.List<CCTouch> touches, CCEvent touchEvent)
        {
            // we only care about the first touch:
            var locationOnScreen = touches[0].Location;
            fishY = locationOnScreen.Y;
        }
    }
}

