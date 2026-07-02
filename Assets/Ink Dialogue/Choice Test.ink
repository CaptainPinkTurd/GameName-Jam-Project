=== choiceKnot ===
<color=\#F8FF39>Hello there</color>
You look[speed=0.5]... 
Interesting[speed=0.5]...[speed=default] Have we ever met somewhere before?
Which pokemon do you wanna choose?
    + [Bulbasaur]
        -> chosen("Bulbasaur")
    + [Charmander]
        -> chosen("Charmander")
    + [Squirtle]
        -> chosen("Squirtle")
        
=== chosen(pokemon) ===
You choose {pokemon}!
Remember to take good care of {pokemon} on your journey, they're gonna be your best companion from now.
-> END