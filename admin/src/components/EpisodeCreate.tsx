import { SimpleForm, Create, TextInput, DateInput } from "react-admin";
import { Box, Grid } from "@mui/material";

interface PodcastCreateProps {
   basePath?: string;
}

export const EpisodeCreate = (props: PodcastCreateProps) => {
   return (
      <Create title="Create a Episode" {...props}>
         <SimpleForm noValidate sanitizeEmptyValues>
            <Box sx={{ flexGrow: 1 }}>
               <Grid container spacing={2}>
                  <Grid item xs={8}>
                     <TextInput source="title" />
                  </Grid>
                  <Grid item xs={4}>
                     <TextInput source="description" multiline />
                  </Grid>
                  <Grid item xs={4}>
                     <TextInput source="tags" />
                  </Grid>
                  <Grid item xs={8}>
                     <TextInput source="posterImg" />
                  </Grid>
               </Grid>
            </Box>
            <TextInput source="podcastId" />
            <TextInput source="title" />

            <TextInput source="audioFileUrl" />
            <TextInput source="videoFileUrl" />
            <TextInput source="length" />
            <TextInput source="views" />
            <TextInput source="publisherId" />
            <DateInput source="createdDate" />
         </SimpleForm>
      </Create>
   );
};

export default EpisodeCreate;
