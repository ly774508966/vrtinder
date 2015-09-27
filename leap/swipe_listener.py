import Leap, sys, thread, time, socket
from datetime import datetime
from Leap import SwipeGesture

class SwipeListener(Leap.Listener):

    state_names = ["STATE_INVALID", "STATE_START", "STATE_UPDATE", "STATE_END"]

    def on_init(self, controller):
        print "Initialized"

    def on_connect(self, controller):
        controller.enable_gesture(Leap.Gesture.TYPE_SWIPE)
        controller.config.set("Gesture.Swipe.MinLength", 10)
        controller.config.set("Gesture.Swipe.MinVelocity", 100)
        controller.config.save()
        self.socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.socket.connect(("address", port))
        print "Connected"

    def on_disconnect(self, controller):
        print "Disconnected"

    def on_exit(self, controller):
        print "Exited"

    def on_frame(self, controller):
        frame = controller.frame()
        for gesture in frame.gestures():
            if gesture.type == Leap.Gesture.TYPE_SWIPE:
                swipe = SwipeGesture(gesture)
                if swipe.direction[0] < 0:
                    print "L", datetime.utcnow().strftime("%H:%M:%S.%f")[:-3]
                    self.socket.send("L")
                else:
                    print "R", datetime.utcnow().strftime("%H:%M:%S.%f")[:-3]
                    self.socket.send("R")
                time.sleep(1)
                return

    def state_string(self, state):
        if state == Leap.Gesture.STATE_START:
            return "STATE_START"
        if state == Leap.Gesture.STATE_UPDATE:
            return "STATE_UPDATE"
        if state == Leap.Gesture.STATE_STOP:
            return "STATE_STOP"
        if state == Leap.Gesture.STATE_INVALID:
            return "STATE_INVALID"

def main():
    listener = SwipeListener()
    controller = Leap.Controller()
    controller.add_listener(listener)
    print "Press enter to quit"
    try:
        sys.stdin.readline()
    except KeyboardInterrupt:
        pass
    finally:
        controller.remove_listener(listener)

if __name__ == "__main__":
    main()
