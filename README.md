# LEVERAGING MACHINE LEARNING TO ASSESS GAME BALANCING FOR COLLECTABLE CARD GAMES

## Summary
This dissertation aims to create a framework for game balancing with suggestion while effectively exploring state space and non-deterministic information. It proposes a framework to assess and bring balance to Collectable Card Games automatically. Collectable Card Games are non-deterministic information games with complex rules. In addition, Collectable Card Games also face the challenge of retain the game balance while new cards are keep introducing and integrating into the existing card pool.
The game-balanced framework consists of three layers. The Player Layer, which includes self-play agent, aim to win this game. The Tournament Layer conducts multiple parallel games between the player’s self-play agent from the player layer and generates data through self-play games. The Game Balance Layer measures the predicted winning probability of each player, and demonstrates game re-balancing using machine learning, predictions and reasoning and then applies a new card to either player to generate perturbation into the game. 
This framework aims to enable efficient identification and suggestion of any numerical parameters that cause imbalance and iterates over game parameters to restore game balance. This framework was designed and tested for a particular game. However, the background methodology can also apply to games such as Magic: The Gathering, Hearthstone, Pokémon Trading Card Game, and Yu-Gi-Oh! Trading Card Game.
Keywords: Collectable Card Games (CCGs), Explainable Artificial Intelligence (XAI), Convolutional Neural Network (CNN), Shapley Additive Explanation (SHAP), Uncertainty Quantification (UQ)

You can read full paper in project's wiki.

## Technology Usage
This dissertation has two parts:
* Game Prototype is designed and developed with Unity
* Game Balancer Framework seperately developed with Python 3 in JupyterLab
