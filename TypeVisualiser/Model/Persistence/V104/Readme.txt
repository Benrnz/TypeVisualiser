******************************************
**** Differences between v1.3 to v1.4 ****
******************************************

Rewrite of layout engine to use a more pure MVVM data binding model.
All diagram elements are now wrapped in a type that includes Show and Co-ordinates. This impacts annotations the most, the TopLeft property is
removed from the class (it is now also wrapped in a diagram element).

