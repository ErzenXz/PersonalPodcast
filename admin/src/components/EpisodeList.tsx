import {
   List,
   Datagrid,
   TextField,
   DateField,
   ImageField,
   EditButton,
   DeleteButton,
} from "react-admin";

interface PodcastListProps {
   basePath?: string | undefined;
}

export const EpisodeList = (props: PodcastListProps) => {
   return (
      <List {...props}>
         <Datagrid>
            <TextField source="id" />
            <TextField source="podcastId" />
            <TextField source="title" />
            <TextField source="description" />
            <TextField source="tags" />
            <ImageField source="posterImg" />
            <TextField source="length" />
            <TextField source="views" />
            <TextField source="publisherId" />
            <DateField source="createdDate" />
            <DateField source="lastUpdate" />
            <EditButton />
            <DeleteButton />
         </Datagrid>
      </List>
   );
};

export default EpisodeList;
