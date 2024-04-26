import { useNavigate } from "react-router-dom";
import "../scss/PodcastCard.scss";
import Podcast from "../types/Podcast";

interface PodcastCardProps {
   podcast?: Podcast;
   loading?: boolean;
}

let goToPodcast = (id: number) => {};

const PodcastCard: React.FC<PodcastCardProps> = ({
   podcast,
   loading = false,
}: PodcastCardProps) => {
   const navigate = useNavigate();

   const formatedDate = (date: string): string => {
      const d = new Date(date);
      return `${d.getDate()}/${d.getMonth() + 1}/${d.getFullYear()}`;
   };

   if (loading || !podcast) {
      return (
         <div className="podcast-card loading-tx">
            <div className="info">
               <div className="title loading-tx"></div>
               <div className="description loading-tx"></div>
               <div className="date loading-tx"></div>
            </div>
            <div className="image loading-tx"></div>
         </div>
      );
   }

   goToPodcast = (id: number) => navigate("/podcast/" + id);

   return (
      <div className="podcast-card" onClick={() => goToPodcast(podcast.id)}>
         <div className="info">
            <h2 className="title">{podcast.title}</h2>
            <p className="description">{podcast.description}</p>
            <p className="date">{formatedDate(podcast.createdDate)}</p>
         </div>
         <div className="image">
            <img src={podcast.posterImg} alt={podcast.title} />
         </div>
      </div>
   );
};

export default PodcastCard;
