# 1. Hierarchical Temporal Memory (HTM) 

The Hierarchical Temporal Memory Cortical Learning Algorithm (HTM CLA) is an algorithm inspired by the biological functioning of the neocortex, which combines spatial pattern recognition and temporal sequence learning(Hawkins, Subutai and Cui, 2017).It organises neurons in layers of column-like units built from many neurons, such that the units are connected into structures called areas. Areas, columns and mini-columns are hierarchically organised (Mountcastle, 1997) and can further be connected in more complex networks, which implement higher cognitive functions like invariant representations, pattern- and sequence-recognition etc. HTM CLA in general consists of two major algorithms: Spatial Pooler and Temporal Memory.The Spatial Pooler operates on mini-columns connected to sensory inputs (Yuwei, Subutai andHawkins, 2017) . It is responsible to learn spatial patterns by encoding the pattern into the sparse distributed representation (SDR). The created SDR, which represents the encoded spatial pattern is further used as the input for the Temporal Memory (TM)algorithm.

- The idea behind reverse engineering the Neocortex consists of the following steps:


### 1a. Sparse Distributed Representations:
Sparse Distributed Representations (SDRs) are binary representation i.e. an array consisting of large number of bits where small percentage are 1’s represents an active neuron and 0 an inactive one. Each bit typically has some meaning (such as the presence of an edge at a particular location and orientation). This means that if two vectors have 1s in the same position they are semantically similar in that attribute.

### 1b. Encoding:
The encoder converts the native format of the data into an SDR that can be fed into an HTM system, it is the in charge of identifying which output bits should be ones and which should be zeros for a particular input value in order to capture the data's significant semantic properties. SDRs with similar input values should have a high degree of overlap.

### 1c. Spatial Pooling:
Spatial pooling Algorithm is for solving problems to represent the input active neurons (comping from sensory or motor organs) to make that the cortex learn the pattern of sequences. It accepts the input vector of different sizes and represents it into sparse vectors of same size(It kind of normalises it). The output of the Spatial Pooler represents the mini columns i.e the pyramidal neuron in the cortices. It posses certain properties like maintaining a fixed sparsity i.e no matter how many bits are on and off in the input the spatial pooler need to maintain the fixed sparsity and to maintain the overlap properties i.e two similar inputs should produce similar outputs in the columns.

The spatial pooler takes the input data and translates the incoming data into active columns. Lets assume that for a given input space a spatial pooling tries to learns the sequences, to learn the sequence every mini column is connected to certain amount of synapses from the input. Then the overlap score is calculated and if the overlap score is above some permeance or threshold value the column is activated else it is not. Let’s say the threshold value is 50 so all the columns with overlap score more than 50 will get activated.

<img src=https://github.com/sandhyabagadi/neocortexapi/blob/Devil_Coders/source/MySEProject/All_images-used/SpacialPooling.png width="300" height="200">

### 1d. Learning:
Learning happens only in those columns of the Spatial Pooler which are active. All the connections that are falling in the input space, the permeance value will increase for them i,e the synaptic connection between them will strengthen while any connections that fall outside of the input space for those the permeance value will be decremented . Learning Spatial Pooler learns better in comparison to the Random Spatial Pooler. This step has been basically derived from the concept ‘Hebbian Learning: Neurons that fire together wire together ’.
For instance, in the diagram given below for any given column in a Spatial Pooler those cell that are connected to the active cells in the input space (i.e which are in green) their permeance will be incremented else those connected outside the input space their permeance will be decremented(the one’s in grey). No inactive column will learn anything.


<img src=https://github.com/sandhyabagadi/neocortexapi/blob/5bbc26be8a0f92203cb1657bf3fe65d3c6beacd8/source/MySEProject/All_images-used/Learning.png width="300" height="200">

### 1e. Boosting:
In order for a column in a Spatial pooler to exist it should be a winning column i.e the overlap score should be above some threshold value while non-winning columns are inhibited from learning. Only the winner columns can update their permanence values. Boosting helps to change the overlap score before the inhibition occurs giving less active columns a better chance to express themselves and diminishing columns that seem overactive . Boosting on better enables the learning of input data i.e it improves the efficiency. In other words we can say that the columns that have low overlap score are boosted so that they can better express themselves and all the columns with higher overlap score are inhibited because they are expressing themselves too much.

### 1f. Temporal Pooling:
Temporal Pooling enables us to understand the sequential pattern over time. It learns the sequences of the active column from the Spatial Pooler and predicts what spatial pattern in coming next based on the temporal context of each input .



# 2 Boosting

In order for a column in a Spatial pooler to exist it should be a winning column i.e the overlap score should be above some threshold value while non-winning columns are inhibited from learning. Only the winner columns can update their permanence values. Boosting helps to change the overlap score before the inhibition occurs giving less active columns a better chance to express themselves and diminishing columns that seem overactive . Boosting on better enables the learning of input data i.e it improves the efficiency. In other words we can say that the columns that have low overlap score are boosted so that they can better express themselves and all the columns with higher overlap score are inhibited because they are expressing themselves too much. 



## 2a Description of Boosting with parameters for more understanding

Boosting can be helpful in driving columns to compete for activation. Boosting is monitored by both the activity and overlap duty cycles Following inhibition, if a column’s active duty cycle falls below the active duty cycles of neighboring columns, then its internal boost factor will increase above one. If a column's active duty cycle arises above the active duty cycles of neighboring columns, its boost factor will decrease below one, This helps drive the competition amongst columns and achieve the spatial pooling goal of using all the columns. Before inhibition, if a column’s overlap duty cycle is below its minimum acceptable value (calculated dynamically as a function of minPctOverlapDutyCycle and the overlap duty cycle of neighboring columns), then all its permanence values are boosted by the increment amount. A subpar duty cycle implies either a column's previously learned inputs are no longer ever active, or the vast majority of them have been "hijacked" by other columns. By raising all synapse permanences in response to a subpar duty cycle before inhibition, we enable a column to search for new inputs.

The HTM CLA provides an algorithm inside of the SP as a computational equivalent of homeostatic plasticity. This algorithm consists of two parts: Synaptic boost of inactive mini-columns and uniform activation of mini-columns.

## Synaptic Boost of inactive mini-columns

A mini-column is defined as inactive (weak) if the number of its connected synapses at the proximal dendrite segment is not sufficient in a learning cycle. If the number of connected synapses of a mini-column in the cycle to the current input is less than the stimulus threshold, then permanence values of all potential synapses of a mini-column will be slightly incremented by parameter si (stimulus increment).

After the permanence values are adapted, permanences of mini-columns with too less connected synapses will be slightly increased. Permanence values of all synapses of the mini-column will be incremented if the number of connected synapses is lower. The value  must be carefully chosen concerning the number of input bits N and the potential radius. In most experiments the value were typically chosen between 20-50% of the number of input bits N. Choosing higher or lower values prevents the SP to converge to the stable state.

- Bump Up Weak Columns [Refer for details](https://github.com/sandhyabagadi/neocortexapi/blob/Devil_Coders/source/MySEProject/Documentation/ReadMe.md)

## Uniform Activation of mini-columns

This implement makes sure that all mini columns in the HTM area become uniformly activated. The absence of this kind of plasticity leads to very different sparsity in the HTM area, which leads to incorrect prediction and inaccurate learning. To ensure the uniform participation of mini columns in the learning, the overall column overlap, and activation are considered. If the value of synaptic permeances defines energy stored in the synaptic connection, the goal here is to keep that energy uniformly distributed across the entire cortical area. 

The following methods contribute to Uniform Activation of mini-columns.

- Update Boost Factors 

- Update Duty Cycles 

- Update Min Duty Cycles 

[Refer for details](https://github.com/sandhyabagadi/neocortexapi/blob/Devil_Coders/source/MySEProject/Documentation/ReadMe.md)




# 3 Important parameters and their usage in spatial pooler and Boosting

|Parameter Name | Meaning |
|--- |--- |
|MAX_BOOST | The Maximum overlap boost factor, Each columns overlap gets multiplied by a boost factor before its considered for inhibition. The actual boost factor for a column is between 1.0 and the maxboost.|
|GLOBAL_INHIBITION | If TRUE global inhibition algorithm will be used. If FALSE local inhibition algorithm will be used.|
|INHIBITION_RADIUS | Defines neighbourhood radius of a column.|
|DUTY_CYCLE_PERIOD | Number of iterations. The period used to calculate duty cycles. Higher values make it take longer to respond to changes in boost. Shorter values make it more unstable and likely to oscillate.|
|WRAP_AROUND | Determines if inputs at the beginning and end of an input dimension should be considered neighbours when mapping columns to inputs.|
|LOCAL_AREA_DENSITY | Density of active columns inside of local inhibition radius. If set on value < 0, explicit number of active columns (NUM_ACTIVE_COLUMNS_PER_INH_AREA) will be used.|
|POTENTIAL_RADIUS | Defines the radius in number of input cells visible to column cells. It is important to choose this value, so every input neuron is connected to at least a single column. For example, if the input has 50000 bits and the column topology is 500, then you must choose some value larger than 50000/500 > 100.|
|MAX_SYNAPSES_PER_SEGMENT | The Maximum number of synapses allowed on a given segment.|
|SYN_PER_BELOW_STIMULUS_INC | Synapses of weak mini-columns will be stimulated by the boosting mechanism. The stimulation is done by adding of this increment value to the current permanence value of the synapse.|
|SYN_PERM_TRIM_THRESHOLD | This value is used by SP. When some permanence is under this value, it is set on zero. In this case the synapse remains the potential one and still can participate in learning. By following structural plasticity principal the synapse would become disconnected from the mini-column.|
|STIMULUS_THRESHOLD | One mini-column is active if its overlap exceeds overlap threshold θo of connected synapses.|
|IS_BUMPUPWEAK_COLOUMNS_DISABLED | Controls bumping-up of weak columns if it is true, weak columns are not updated and if it is false bumping up of weak columns take place|
|POTENTIAL_PCT | Defines the percent of of inputs within potential radius, which can/should be connected to the column.|
|UPDATE_ACTIVE_DUTYCYCLE | Computes a moving average of how often column c has been active after inhibition.|
|UPDATE_OVERLAPE_DUTY_CYCLE | Computes a moving average of how often column c has overlap greater than stimulusThreshold.| 
|ACTIVE_DUTY_CYCLE | A moving average denoting the frequency of column activation.|
|OVERLAP_DUTY_CYCLE | A moving average denoting the frequency of the column’s overlap value being at least equal to the proximal segment activation threshold.|

# 4 References

[1] Article on understanding HTM: https://medium.com/@rockingrichie1994/understanding-hierarchal-temporal-memory-f6a1be38e07e

[2] D. Dobric, "Github," [Online]. Available: https://github.com/ddobric/neocortexapi

[3] Improved HTM Spatial Pooler with Homeostatic Plasticity Control Author: Damir Dobric, Andreas Pech, Bogdan Ghitaand Thomas Wennekers. https://www.academia.edu/45090289/Improved_HTM_Spatial_Pooler_with_Homeostatic_Plasticity_control

[4] Deep Learning Author: Mariette Awad, Rahul Khanna https://link.springer.com/chapter/10.1007/978-1-4302-5990-9_9

