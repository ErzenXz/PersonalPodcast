import "../scss/Model.scss";

function Modal({ onClose, children }: { onClose: () => void; children: React.ReactNode }) {
   return (
      <div className="modal-backdrop">
         <div className="modal-content">
            <button className="close-button" onClick={onClose}>
               ×
            </button>
            {children}
         </div>
      </div>
   );
}

export default Modal;
