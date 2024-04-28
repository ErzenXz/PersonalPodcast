import { useEffect, useState } from "react";
import ReactDOM from "react-dom";
import "../scss/Popup.scss";

const Popup = ({
   message,
   duration = 4500,
   delay = 0,
   update = true,
}: {
   message: string;
   duration?: number;
   delay?: number;
   update?: boolean;
}) => {
   const [isVisible, setIsVisible] = useState(true);

   useEffect(() => {
      setIsVisible(true);

      const timer = setTimeout(() => {
         setIsVisible(false);
      }, duration + delay);

      return () => clearTimeout(timer);
   }, [duration, delay, update]);

   useEffect(() => {
      if (!isVisible) {
         const container = document.getElementById("popup-container");
         if (container) {
            container.style.animationName = "popFromBottom";
         }

         const fadeOutTimer = setTimeout(() => {
            if (container) {
               ReactDOM.unmountComponentAtNode(container);
            }
         }, 300);

         return () => clearTimeout(fadeOutTimer);
      }
   }, [isVisible]);

   useEffect(() => {
      if (!isVisible) {
         const container = document.getElementById("popup-container");
         if (container) {
            // Apply the transition just before starting the animation
            container.style.opacity = "0";
            container.style.transition = "opacity 0.3s";
         }
      }
   }, [isVisible, update]);

   if (!isVisible) return null;

   return (
      <div className="toast" id="popup-container">
         <span>{message}</span>
      </div>
   );
};

export default Popup;
