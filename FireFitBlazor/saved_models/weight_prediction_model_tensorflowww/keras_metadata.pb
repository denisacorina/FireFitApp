
á'root"_tf_keras_layer*Á&{
  "name": "model",
  "trainable": true,
  "expects_training_arg": false,
  "dtype": "float32",
  "batch_input_shape": null,
  "autocast": false,
  "class_name": "Functional",
  "config": {
    "name": "model",
    "layers": [
      {
        "name": "input_1",
        "class_name": "InputLayer",
        "config": {
          "sparse": false,
          "ragged": false,
          "name": "input_1",
          "dtype": "float32",
          "batch_input_shape": {
            "class_name": "TensorShape",
            "items": [
              null,
              7,
              3
            ]
          }
        },
        "inbound_nodes": []
      },
      {
        "name": "lstm",
        "class_name": "LSTM",
        "config": {
          "return_sequences": true,
          "return_state": false,
          "go_backwards": false,
          "stateful": false,
          "unroll": false,
          "time_major": false,
          "units": 32,
          "activation": "tanh",
          "recurrent_activation": "sigmoid",
          "use_bias": true,
          "dropout": 0.0,
          "zero_output_for_mask": false,
          "recurrent_dropout": 0.0,
          "name": null,
          "dtype": "float32",
          "trainable": true
        },
        "inbound_nodes": [
          [
            "input_1",
            0,
            0
          ]
        ]
      },
      {
        "name": "dropout",
        "class_name": "Dropout",
        "config": {
          "rate": 0.2,
          "noise_shape": null,
          "seed": null,
          "name": null,
          "dtype": "float32",
          "trainable": true
        },
        "inbound_nodes": [
          [
            "lstm",
            0,
            0
          ]
        ]
      },
      {
        "name": "lstm_1",
        "class_name": "LSTM",
        "config": {
          "return_sequences": false,
          "return_state": false,
          "go_backwards": false,
          "stateful": false,
          "unroll": false,
          "time_major": false,
          "units": 16,
          "activation": "tanh",
          "recurrent_activation": "sigmoid",
          "use_bias": true,
          "dropout": 0.0,
          "zero_output_for_mask": false,
          "recurrent_dropout": 0.0,
          "name": null,
          "dtype": "float32",
          "trainable": true
        },
        "inbound_nodes": [
          [
            "dropout",
            0,
            0
          ]
        ]
      },
      {
        "name": "dense",
        "class_name": "Dense",
        "config": {
          "units": 8,
          "activation": "relu",
          "use_bias": true,
          "kernel_initializer": {
            "class_name": "GlorotUniform",
            "config": {
              "seed": null
            }
          },
          "bias_initializer": {
            "class_name": "Zeros",
            "config": {}
          },
          "kernel_regularizer": null,
          "bias_regularizer": null,
          "kernel_constraint": null,
          "bias_constraint": null,
          "name": null,
          "dtype": "float32",
          "trainable": true
        },
        "inbound_nodes": [
          [
            "lstm_1",
            0,
            0
          ]
        ]
      },
      {
        "name": "dropout_1",
        "class_name": "Dropout",
        "config": {
          "rate": 0.2,
          "noise_shape": null,
          "seed": null,
          "name": null,
          "dtype": "float32",
          "trainable": true
        },
        "inbound_nodes": [
          [
            "dense",
            0,
            0
          ]
        ]
      },
      {
        "name": "dense_1",
        "class_name": "Dense",
        "config": {
          "units": 1,
          "activation": "linear",
          "use_bias": true,
          "kernel_initializer": {
            "class_name": "GlorotUniform",
            "config": {
              "seed": null
            }
          },
          "bias_initializer": {
            "class_name": "Zeros",
            "config": {}
          },
          "kernel_regularizer": null,
          "bias_regularizer": null,
          "kernel_constraint": null,
          "bias_constraint": null,
          "name": null,
          "dtype": "float32",
          "trainable": true
        },
        "inbound_nodes": [
          [
            "dropout_1",
            0,
            0
          ]
        ]
      }
    ],
    "input_layers": [
      [
        "input_1",
        0,
        0
      ]
    ],
    "output_layers": [
      [
        "dense_1",
        0,
        0
      ]
    ]
  },
  "shared_object_id": 1,
  "build_input_shape": {
    "class_name": "TensorShape",
    "items": [
      null,
      7,
      3
    ]
  }
}2
‡root.layer-0"_tf_keras_input_layer*∞{"class_name":"InputLayer","name":"input_1","dtype":"float32","sparse":false,"ragged":false,"batch_input_shape":{"class_name":"TensorShape","items":[null,7,3]},"config":{"sparse":false,"ragged":false,"name":"input_1","dtype":"float32","batch_input_shape":{"class_name":"TensorShape","items":[null,7,3]}}}2
∑root.layer_with_weights-0"_tf_keras_layer*Ä{
  "name": "lstm",
  "trainable": true,
  "expects_training_arg": false,
  "dtype": "float32",
  "batch_input_shape": null,
  "autocast": false,
  "class_name": "LSTM",
  "config": {
    "return_sequences": true,
    "return_state": false,
    "go_backwards": false,
    "stateful": false,
    "unroll": false,
    "time_major": false,
    "units": 32,
    "activation": "tanh",
    "recurrent_activation": "sigmoid",
    "use_bias": true,
    "dropout": 0.0,
    "zero_output_for_mask": false,
    "recurrent_dropout": 0.0,
    "name": null,
    "dtype": "float32",
    "trainable": true
  },
  "shared_object_id": 2,
  "build_input_shape": {
    "class_name": "TensorShape",
    "items": [
      null,
      7,
      3
    ]
  }
}2
çroot.layer-2"_tf_keras_layer*„{
  "name": "dropout",
  "trainable": true,
  "expects_training_arg": false,
  "dtype": "float32",
  "batch_input_shape": null,
  "autocast": false,
  "class_name": "Dropout",
  "config": {
    "rate": 0.2,
    "noise_shape": null,
    "seed": null,
    "name": null,
    "dtype": "float32",
    "trainable": true
  },
  "shared_object_id": 3,
  "build_input_shape": {
    "class_name": "TensorShape",
    "items": [
      null,
      7,
      32
    ]
  }
}2
ªroot.layer_with_weights-1"_tf_keras_layer*Ñ{
  "name": "lstm_1",
  "trainable": true,
  "expects_training_arg": false,
  "dtype": "float32",
  "batch_input_shape": null,
  "autocast": false,
  "class_name": "LSTM",
  "config": {
    "return_sequences": false,
    "return_state": false,
    "go_backwards": false,
    "stateful": false,
    "unroll": false,
    "time_major": false,
    "units": 16,
    "activation": "tanh",
    "recurrent_activation": "sigmoid",
    "use_bias": true,
    "dropout": 0.0,
    "zero_output_for_mask": false,
    "recurrent_dropout": 0.0,
    "name": null,
    "dtype": "float32",
    "trainable": true
  },
  "shared_object_id": 4,
  "build_input_shape": {
    "class_name": "TensorShape",
    "items": [
      null,
      7,
      32
    ]
  }
}2
Ároot.layer_with_weights-2"_tf_keras_layer*∞{
  "name": "dense",
  "trainable": true,
  "expects_training_arg": false,
  "dtype": "float32",
  "batch_input_shape": null,
  "autocast": false,
  "input_spec": {
    "class_name": "InputSpec",
    "config": {
      "DType": null,
      "Shape": null,
      "Ndim": null,
      "MinNdim": 2,
      "MaxNdim": null,
      "Axes": {
        "-1": 16
      }
    },
    "shared_object_id": 5
  },
  "class_name": "Dense",
  "config": {
    "units": 8,
    "activation": "relu",
    "use_bias": true,
    "kernel_initializer": {
      "class_name": "GlorotUniform",
      "config": {
        "seed": null
      }
    },
    "bias_initializer": {
      "class_name": "Zeros",
      "config": {}
    },
    "kernel_regularizer": null,
    "bias_regularizer": null,
    "kernel_constraint": null,
    "bias_constraint": null,
    "name": null,
    "dtype": "float32",
    "trainable": true
  },
  "shared_object_id": 6,
  "build_input_shape": {
    "class_name": "TensorShape",
    "items": [
      null,
      16
    ]
  }
}2
Ñroot.layer-5"_tf_keras_layer*⁄{
  "name": "dropout_1",
  "trainable": true,
  "expects_training_arg": false,
  "dtype": "float32",
  "batch_input_shape": null,
  "autocast": false,
  "class_name": "Dropout",
  "config": {
    "rate": 0.2,
    "noise_shape": null,
    "seed": null,
    "name": null,
    "dtype": "float32",
    "trainable": true
  },
  "shared_object_id": 7,
  "build_input_shape": {
    "class_name": "TensorShape",
    "items": [
      null,
      8
    ]
  }
}2
Èroot.layer_with_weights-3"_tf_keras_layer*≤{
  "name": "dense_1",
  "trainable": true,
  "expects_training_arg": false,
  "dtype": "float32",
  "batch_input_shape": null,
  "autocast": false,
  "input_spec": {
    "class_name": "InputSpec",
    "config": {
      "DType": null,
      "Shape": null,
      "Ndim": null,
      "MinNdim": 2,
      "MaxNdim": null,
      "Axes": {
        "-1": 8
      }
    },
    "shared_object_id": 8
  },
  "class_name": "Dense",
  "config": {
    "units": 1,
    "activation": "linear",
    "use_bias": true,
    "kernel_initializer": {
      "class_name": "GlorotUniform",
      "config": {
        "seed": null
      }
    },
    "bias_initializer": {
      "class_name": "Zeros",
      "config": {}
    },
    "kernel_regularizer": null,
    "bias_regularizer": null,
    "kernel_constraint": null,
    "bias_constraint": null,
    "name": null,
    "dtype": "float32",
    "trainable": true
  },
  "shared_object_id": 9,
  "build_input_shape": {
    "class_name": "TensorShape",
    "items": [
      null,
      8
    ]
  }
}2