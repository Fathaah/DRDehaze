import tensorflow as tf
from metrics import psnr


def get_model():
  '''
  Make all the model edits here
  '''
  x = tf.keras.Input((None, None, 3))
  inputs = x
  #down sampling  This is the reducing dimesnionality part, like the left side
  #This part is also called the decoder path
  f = 8 #Filter size
  layers = [] #Storing layers here
  '''
  This for loop is adding convolutional layers to be exact in each iteration 
  its adding 2 conv layers and appending it to the layers list. Then a maxpooling 
  before going to the next layer.
  '''
  for i in range(0, 6):
    x = tf.keras.layers.Conv2D(f, 3, activation='relu', padding='same') (x)
    x = tf.keras.layers.Conv2D(f, 3, activation='relu', padding='same') (x)
    layers.append(x)
    x = tf.keras.layers.MaxPooling2D() (x)
    x = tf.keras.layers.BatchNormalization()(x)
    f = f*2
  ff2 = 64 
  '''
  This is the middle part where your taking the output from the previous part 
  and then starting the process of rebiulding the image.
  '''
  #bottleneck 
  j = len(layers) - 1
  x = tf.keras.layers.Conv2D(f, 3, activation='elu', padding='same') (x)
  x = tf.keras.layers.Conv2D(f, 3, activation='elu', padding='same') (x)
  x = tf.keras.layers.BatchNormalization()(x)
  x = tf.keras.layers.Conv2DTranspose(ff2, 2, strides=(2, 2), padding='same') (x) #This is doing conv in reverse
  x = tf.keras.layers.Concatenate(axis=3)([x, layers[j]]) #This si the sinterconnection, concatenation.
  j = j -1 

  #upsampling 
  #The decoder part, doing some convolutions, then building it back up
  #Conv2DTranspose for rebuilding the image
  #Concatinating the output and the jth layer for intercconection
  for i in range(0, 5):
    ff2 = ff2//2
    f = f // 2 
    x = tf.keras.layers.Conv2D(f, 3, activation='elu', padding='same') (x)
    x = tf.keras.layers.Conv2D(f, 3, activation='elu', padding='same') (x)
    x = tf.keras.layers.BatchNormalization()(x)
    x = tf.keras.layers.Conv2DTranspose(ff2, 2, strides=(2, 2), padding='same') (x)
    x = tf.keras.layers.Concatenate(axis=3)([x, layers[j]])
    j = j -1 
    
  #Reconstructing the input back
  x = tf.keras.layers.BatchNormalization()(x)
  x = tf.keras.layers.Conv2D(f, 3, activation='elu', padding='same') (x)
  x = tf.keras.layers.Conv2D(3, 3, activation='tanh', padding='same') (x)
  outputs = tf.keras.layers.Add()([inputs, x])
  #model creation 
  model = tf.keras.models.Model(inputs=[inputs], outputs=[outputs])
  model.compile(optimizer = 'adam', loss = tf.keras.losses.MSE, metrics=[psnr])

  return model