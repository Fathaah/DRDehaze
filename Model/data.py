import tensorflow as tf
import os
from options import dir

path_label = os.path.join(dir, 'Y' + os.sep)

def create_datagen(files):
  list_ds = tf.data.Dataset.from_tensor_slices(files)
  images_ds = list_ds.map(parse_image)
  return images_ds
  
@tf.function
def parse_image(filename, verbose = False, test = False, img_shape = [256, 256]):
    #tf.print(filename)
#     if tf.strings.length(filename) < 12:

    def augment(img, clr):
        
        if tf.random.uniform([]) < 0.75:
            if tf.random.uniform([]) < 0.5:
                return tf.image.flip_left_right(img), tf.image.flip_left_right(clr)
            else:
                return tf.image.resize(tf.image.central_crop(img, central_fraction=0.5), [256,256]), tf.image.resize(tf.image.central_crop(clr, central_fraction=0.5), [256,256])
        else:
            return (img), (clr)

    if test:
        image = tf.io.read_file(filename)
        image = tf.image.decode_jpeg(image, channels=3) 
        image = tf.image.convert_image_dtype(image, tf.float32)         
        return tf.image.rgb_to_yuv(image)
    parts = tf.strings.split(filename, os.sep)
    parts = tf.strings.split(parts[-1], '_')
    image = tf.io.read_file(filename)
    image = (tf.image.decode_jpeg(image, channels=3))
    image = ((tf.image.convert_image_dtype(image, tf.float32)))        
    image = tf.image.resize(image, img_shape, method=tf.image.ResizeMethod.AREA, antialias=True)
    #tf.print('.\\Y\\' + parts[0] + '.png')
    clr_image = tf.io.read_file(path_label + parts[0] + '.png')
    clr_image = tf.image.decode_jpeg(clr_image, channels=3)
    clr_image = (tf.image.convert_image_dtype(clr_image, tf.float32))
    clr_image = tf.image.resize(clr_image, img_shape, method=tf.image.ResizeMethod.AREA,antialias=True) 
    return augment(tf.image.rgb_to_yuv(image), tf.image.rgb_to_yuv(clr_image))