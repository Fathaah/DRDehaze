
import tensorflow as tf
import tensorflow.keras.backend as K

effnet = tf.keras.applications.EfficientNetB3(include_top=False, weights='imagenet', input_tensor=None, input_shape=None,pooling=None)
MeanSquaredError=tf.keras.losses.MeanSquaredError
class MSEContent(tf.keras.losses.Loss):
  def call(self, y_true, y_pred):
    y_pred = tf.keras.applications.efficientnet.preprocess_input(y_pred)
    y_true = tf.keras.applications.efficientnet.preprocess_input(y_true)
    sr_features = effnet(y_pred) / 12.75
    hr_features = effnet(y_true) / 12.75
    return  tf.math.reduce_mean(tf.square(y_true - y_pred)) + 0.01 * tf.math.reduce_mean(tf.square(sr_features - hr_features))
def psnr(y_true, y_pred):
    max_pixel = 1.0
    return (10.0 * K.log((max_pixel ** 2) / (K.mean(K.square(y_pred - y_true) + 1e-6, axis=-1)))) / 2.303