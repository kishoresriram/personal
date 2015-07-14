package com.example.kishore.postpics;

import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.net.Uri;
import android.os.Environment;
import android.provider.MediaStore;
import android.support.v7.app.ActionBarActivity;
import android.os.Bundle;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.HorizontalScrollView;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.ListView;
import android.widget.RelativeLayout;
import android.widget.TextView;
import android.widget.Toast;

import java.io.File;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.Locale;


public class MainActivity extends ActionBarActivity {

    //private ImageView imageView;
    private Button btnCaptureImage;
    private Uri fileUri;
    private LayoutInflater layoutInflater;
    private RelativeLayout relativeLayout;
    //private HorizontalScrollView horizontalView;
    private LinearLayout linearLayout;

    // Activity request codes
    private static final int CAMERA_CAPTURE_IMAGE_REQUEST_CODE = 100;
    public static final int MEDIA_TYPE_IMAGE = 1;
    private static final String IMAGE_DIRECTORY_NAME = "InfyGram";


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        btnCaptureImage = (Button) findViewById(R.id.btnCaptureImage);

        btnCaptureImage.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                clickImage();
            }
        });

    }

    private void clickImage() {
        Intent intent = new Intent(MediaStore.ACTION_IMAGE_CAPTURE);
        fileUri = getOutputMediaFileUri(MEDIA_TYPE_IMAGE);//user defined method to return

        intent.putExtra(MediaStore.EXTRA_OUTPUT,fileUri);

        startActivityForResult(intent, CAMERA_CAPTURE_IMAGE_REQUEST_CODE);
    }

    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        if (requestCode == CAMERA_CAPTURE_IMAGE_REQUEST_CODE) {
            if (resultCode == RESULT_OK) {
                // successfully captured the image
                // display it in image view
                previewCapturedImage();
            } else if (resultCode == RESULT_CANCELED) {
                // user cancelled Image capture
                Toast.makeText(getApplicationContext(),
                        "User cancelled image capture", Toast.LENGTH_SHORT)
                        .show();
            } else {
                // failed to capture image
                Toast.makeText(getApplicationContext(),
                        "Sorry! Failed to capture image", Toast.LENGTH_SHORT)
                        .show();
            }
        }
    }

    private void previewCapturedImage() {
        try{
            layoutInflater = getLayoutInflater();
            relativeLayout = (RelativeLayout) layoutInflater.inflate(R.layout.relative_layout_singleitem, null);


            //final TextView postDescription = (TextView) relativeLayout.findViewById(R.id.picDescription);
            TextView postDate = (TextView) relativeLayout.findViewById(R.id.picDate);
            ImageView imageView = (ImageView) relativeLayout.findViewById(R.id.imageView);

            String timeStamp = new SimpleDateFormat("HH:mm",
                    Locale.getDefault()).format(new Date());
            postDate.setText(timeStamp);
            //postDescription.setText("Snap");


            //imageView.setVisibility(View.VISIBLE);

            //RelativeLayout relativeLayout = new RelativeLayout(this);


            imageView.setVisibility(View.VISIBLE);

            //BitmapFactory.Options options = new BitmapFactory.Options();

            //options.inSampleSize = 8;

            //final Bitmap bitmap = BitmapFactory.decodeFile(fileUri.getPath());

            //kishore

            Bitmap bitmap;

            BitmapFactory.Options bounds = new BitmapFactory.Options();
            bounds.inJustDecodeBounds = true;
            BitmapFactory.decodeFile(fileUri.getPath(), bounds);

            if (bounds.outWidth == -1) { // TODO: Error }
                int width = bounds.outWidth;
                int height = bounds.outHeight;
                //boolean withinBounds = width <= maxWidth && height <= maxHeight;
            }
            bitmap = Bitmap.createScaledBitmap(BitmapFactory.decodeFile(fileUri.getPath()),900, 960, true);
            //mPicture = new ImageView(context);
            //mPicture.setImageBitmap(bm);

            //kishore

            imageView.setImageBitmap(bitmap);

            //horizontalView = (HorizontalScrollView) findViewById(R.id.horizontalView);
            linearLayout = (LinearLayout) findViewById(R.id.innerLinearLayout);
            linearLayout.addView(relativeLayout);

        }
        catch (Exception e)
        {
            e.printStackTrace();
            Log.d("Message - ",e.getMessage());
        }
    }


    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.menu_main, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        // Handle action bar item clicks here. The action bar will
        // automatically handle clicks on the Home/Up button, so long
        // as you specify a parent activity in AndroidManifest.xml.
        int id = item.getItemId();

        //noinspection SimplifiableIfStatement
        if (id == R.id.action_settings) {
            return true;
        }

        return super.onOptionsItemSelected(item);
    }


    //Helper Methods --

    /**
     * Creating file uri to store image/video
     */
    public Uri getOutputMediaFileUri(int type)
    {
        return Uri.fromFile(getOutputMediaFile(type));
    }
    /*
     * returning image / video
     */
    private static File getOutputMediaFile(int type) {

        // External sdcard location
        File mediaStorageDir = new File(Environment.getExternalStoragePublicDirectory(Environment.DIRECTORY_PICTURES)+ File.separator + IMAGE_DIRECTORY_NAME);

        // Create the storage directory if it does not exist
        if (!mediaStorageDir.exists()) {
            if (!mediaStorageDir.mkdirs()) {
                Log.d(IMAGE_DIRECTORY_NAME, "Oops! Failed create "
                        + IMAGE_DIRECTORY_NAME + " directory");
                return null;
            }
        }

        // Create a media file name
        String timeStamp = new SimpleDateFormat("yyyyMMdd_HHmmss",
                Locale.getDefault()).format(new Date());
        File mediaFile;
        if (type == MEDIA_TYPE_IMAGE) {
            mediaFile = new File(mediaStorageDir.getPath() + File.separator
                    + "IMG_" + timeStamp + ".jpg");
        }
        else {
            return null;
        }

        return mediaFile;
    }


}
