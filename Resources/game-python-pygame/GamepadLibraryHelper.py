﻿
def libhelper_gamepad_get_current_joystick_count():
  return pygame.joystick.get_count()

def libhelper_gamepad_get_joystick(index):
  joystick = pygame.joystick.Joystick(index)
  joystick.init()
  return joystick

def libhelper_gamepad_get_joystick_name(joystick):
  return joystick.get_name()

def libhelper_gamepad_get_joystick_button_count(joystick):
  return joystick.get_numbuttons()

def libhelper_gamepad_get_joystick_axis_1d_count(joystick):
  return joystick.get_numaxes()

def libhelper_gamepad_get_joystick_axis_2d_count(joystick):
  return joystick.get_numhats()

def libhelper_gamepad_get_joystick_button_state(joystick, index):
  return joystick.get_button(index)

def libhelper_gamepad_get_joystick_axis_1d_state(joystick, index):
  return joystick.get_axis(index)

def libhelper_gamepad_get_joystick_axis_2d_state(joystick, index, xyOutParams):
  xy = joystick.get_hat(index)
  xyOutParams[0] = xy[0]
  xyOutParams[1] = xy[1]
