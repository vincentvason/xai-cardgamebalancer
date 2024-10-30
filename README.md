# LEVERAGING MACHINE LEARNING TO ASSESS GAME BALANCING FOR COLLECTABLE CARD GAMES

## Content
This project includes:
* Project Overview: You can watch on [YouTube](https://www.youtube.com/watch?v=ujCheuUMxXE) with a timestamp on each chapter. 
* Dissertation: Please visit [project's wiki](https://github.com/vincentvason/xai-cardgamebalancer/wiki).
* Game Prototype: Please visit [itch.io](https://vincentvason.itch.io/spellbot) for a demo and a [game project](https://github.com/vincentvason/xai-cardgamebalancer/tree/main/GameProject/Spellbot).
* Game Balancing Framework: Please visit a [framework prototype](https://github.com/vincentvason/xai-cardgamebalancer/tree/main/FrameworkPrototype).

## Summary
This dissertation aims to create a framework for game balancing with suggestion while effectively exploring state space and non-deterministic information. It proposes a framework to assess and bring balance to Collectable Card Games automatically. Collectable Card Games are non-deterministic information games with complex rules. In addition, Collectable Card Games also face the challenge of retain the game balance while new cards are keep introducing and integrating into the existing card pool.
The game-balanced framework consists of three layers. The Player Layer, which includes self-play agent, aim to win this game. The Tournament Layer conducts multiple parallel games between the player’s self-play agent from the player layer and generates data through self-play games. The Game Balance Layer measures the predicted winning probability of each player, and demonstrates game re-balancing using machine learning, predictions and reasoning and then applies a new card to either player to generate perturbation into the game. 
This framework aims to enable efficient identification and suggestion of any numerical parameters that cause imbalance and iterates over game parameters to restore game balance. This framework was designed and tested for a particular game. However, the background methodology can also apply to games such as Magic: The Gathering, Hearthstone, Pokémon Trading Card Game, and Yu-Gi-Oh! Trading Card Game.
Keywords: Collectable Card Games (CCGs), Explainable Artificial Intelligence (XAI), Convolutional Neural Network (CNN), Shapley Additive Explanation (SHAP), Uncertainty Quantification (UQ)

Please visit [project's wiki](https://github.com/vincentvason/xai-cardgamebalancer/wiki) for full paper.

## Technology Usage
This dissertation has two parts:
* Game Prototype is designed and developed with C# .NET Unity (Prototype is playable in [itch.io](https://vincentvason.itch.io/spellbot).)
* Game Balancer Framework seperately developed with Python 3 in JupyterLab

## Disclaimer
This dissertation is submitted by [Vason Maitree](https://www.linkedin.com/in/vasonmi3/) and supervised by [Dr. Nikolaos Ersotelos](https://people.uwe.ac.uk/Person/NikolaosErsotelos).

[MSc Commerical Games Development, University of West of England](https://info.uwe.ac.uk/programmes/displayentry.asp?code=I60012&rp=listEntry.asp)

