import {
   Show,
   SimpleShowLayout,
   TextField,
   DateField,
   RichTextField,
   ImageField,
} from "react-admin";

export const ShowEpisode = () => (
   <Show>
      <SimpleShowLayout>
         <TextField source="id" />
         <TextField source="podcastId" />
         <TextField source="title" />
         <RichTextField source="description" />
         <TextField source="tags" />
         <ImageField source="posterImg" />
         <TextField source="length" />
         <TextField source="views" />
         <TextField source="publisherId" />
         <DateField source="createdDate" />
         <DateField source="lastUpdate" />
      </SimpleShowLayout>
   </Show>
);

export default ShowEpisode;
