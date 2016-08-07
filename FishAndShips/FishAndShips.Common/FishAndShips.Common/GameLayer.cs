using System;
using System.Collections.Generic;
using CocosSharp;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using System.Linq;

namespace FishAndShips.Common
{
    public class GameLayer : CCLayerColor
    {
        CCSprite fishSprite;
        CCSprite netSprite;
        CCSprite shipSprite;
        CCLabel scoreLabel;
        long gameStartTimeMillisecounds;
        long previousFrameMillisecounds;
        double fishCenterYCoordinate = Constants.Height / 2;
        CCDrawNode drawNode;
        bool gameStarted = false;
        float fishAmplitude = Constants.Height / 2;
        double frequency = 1d;
        double _frequency;
        double phase = 0.0f;

        int score;

        public GameLayer() : base(CCColor4B.Black)
        {
            Init();            
        }

        public void Init()
        {
            _frequency = frequency;
            // "paddle" refers to the paddle.png image
            fishSprite = new CCSprite("fish");
            fishSprite.PositionX = 70;
            fishSprite.PositionY = (float)fishCenterYCoordinate;
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
            scoreLabel.PositionX = Constants.Width - 500;
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

            //if (collision || collision2)
            if(false)
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
                long currentTimeMillisecounds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                netSprite.PositionX = Constants.Width - (((currentTimeMillisecounds - gameStartTimeMillisecounds) / 2) % Constants.Width);
                shipSprite.PositionX = Constants.Width - (((currentTimeMillisecounds - gameStartTimeMillisecounds) / 3) % Constants.Width);

                //drawNode.Clear();
                //drawNode.DrawLine(new CCPoint(0, fishCenterYCoordinate), new CCPoint(Constants.Width, fishCenterYCoordinate), new CCColor4B(100, 100, 100));
                //drawNode.DrawLine(new CCPoint(0, fishCenterYCoordinate - fishAmplitude), new CCPoint(Constants.Width, fishCenterYCoordinate - fishAmplitude), new CCColor4B(139, 0, 0));
                //drawNode.DrawLine(new CCPoint(0, fishCenterYCoordinate + fishAmplitude), new CCPoint(Constants.Width, fishCenterYCoordinate + fishAmplitude), new CCColor4B(139, 0, 0));

                if (frequency != _frequency)
                    CalcNewFreq(currentTimeMillisecounds);

                fishSprite.PositionY = (float)(fishCenterYCoordinate + (double)((Math.Sin((_frequency * currentTimeMillisecounds / 700d) + phase)) * fishAmplitude));
                if (fishSprite.PositionY < 0)
                    fishSprite.PositionY = 0;
                if (fishSprite.PositionY > Constants.Height)
                    fishSprite.PositionY = Constants.Height;

                score = (int)((currentTimeMillisecounds - gameStartTimeMillisecounds) / 1000);
                if(frequency > 1d)
                {
                    frequency -= ((currentTimeMillisecounds - previousFrameMillisecounds) / 10000f);
                    
                    if (frequency < 1d)
                        frequency = 1d;

                    Debug.WriteLine(frequency);
                }
                scoreLabel.Text = $"Score: {score} Speed: {100 + (int)(100d * frequency)}";
                previousFrameMillisecounds = currentTimeMillisecounds;
            }
        }

        void CalcNewFreq(long time)
        {
            double curr = (time * _frequency + phase) % (2.0f * Math.PI);
            double next = (time * frequency) % (2.0f * Math.PI);
            phase = curr - next;
            _frequency = frequency;
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
                    previousFrameMillisecounds = gameStartTimeMillisecounds;
                    Schedule(RunGameLogic);
                    gameStarted = true;
                }
            }
        }

        void HandleTouchesMoved(System.Collections.Generic.List<CCTouch> touches, CCEvent touchEvent)
        {
            if (touches.Count == 1)
            {
                frequency += 0.5d;
                if (frequency > 2.5d)
                    frequency = 2.5d;
            }          
        }
    }
}

