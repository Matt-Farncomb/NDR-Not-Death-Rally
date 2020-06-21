TODO:

Agents don't go back when they have gone the right way, but it is too far - they shoudl always be ordered by cost
So maybe at the moment they jsut go with whatver way gets them there in there end.

Weird error with car(1) how it goes off map somehow. Myabe some kind of indexing error... Or this is connected to the creations of path.
IF path is simply wrong, it means the parentsd are fucked.
Parents being added wrong would also explain why they dont keep the shortest path. 

Makes sense. Must be the parents.

//COROUTINES:
//I think I can use them
//But the individual computations were very big
// So it would do a big computation, then go straight ot the next frame. Then do it again.
// I thnk I can like "do big computation" then wait for 2 seconds or whatver, then go on to the next iteration, pause again etc.
// Thr porblem was the computations from the pathfinding was taking seconds for each frame.