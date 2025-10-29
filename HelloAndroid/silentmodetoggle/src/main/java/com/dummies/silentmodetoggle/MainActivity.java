package com.dummies.silentmodetoggle;

import android.app.Activity;
import android.app.NotificationManager;
import android.content.Context;
import android.content.Intent;
import android.provider.MediaStore.Audio;
import android.media.AudioManager;
import android.os.Build;
import android.os.Bundle;
import android.provider.Settings;
import android.util.Log;
import android.view.View;
import android.widget.FrameLayout;
import android.widget.ImageView;

import androidx.annotation.RequiresApi;

import com.dummies.silentmodetoggle.util.RingerHelper;

public class MainActivity extends Activity {

    AudioManager audioManager;

    /**
     * This method is called to initialize the activity after the
     * java constructor for this class has been called.  This is
     * typically where you would call setContentView to inflate your
     * layout, and findViewById to initialize your views.
     * @param savedInstanceState contains additional data about the
     *                           saved state of the activity,
     *                           if it was previously shutdown and is
     *                           now being re-created from saved state.
     */
    @RequiresApi(api = Build.VERSION_CODES.M)
    @Override
    public void onCreate(Bundle savedInstanceState) {
        // Always call super.onCreate() first.
        super.onCreate(savedInstanceState);
        // Get a reference to Android's AudioManager so we can use
        // it to toggle our ringer.
        audioManager = (AudioManager) getSystemService(AUDIO_SERVICE);
        NotificationManager notificationManager = (NotificationManager)getSystemService(Context.NOTIFICATION_SERVICE);

        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.N
                && !notificationManager.isNotificationPolicyAccessGranted()) {
            Intent intent = new Intent(Settings.ACTION_NOTIFICATION_POLICY_ACCESS_SETTINGS);
            this.startActivity(intent);
        }
        // Initialize our layout using the res/layout/activity_main.xml
        // layout file that contains our views for this activity.
        setContentView(R.layout.activity_main);

        //Find the view with the ID "content" in layout file
        FrameLayout contentView =
                (FrameLayout) findViewById(R.id.content);

        // Create a click listener for the contentView that will toggle
        // the phone's ringer state, and then update the UI to reflect
        // the new state.
        // The astute reader might wonder why we don't set a click
        // listener on the image itself instead of on the surrounding
        // frame.  There are two reasons for this:
        //   1. We want the whole page to be clickable instead of just
        //      the image.
        //   2. We use android:foreground with a value of
        //      ?android:attr/selectableItemBackground to create a
        //      pretty ripple effect when we click on the page.  For
        //      technical reasons, it is not possible to use this same
        //      technique on an ImageView.
        contentView.setOnClickListener(new View.OnClickListener() {
            public void onClick(View v) {

                // Toggle the ringer mode.  If it's currently normal,
                // make it silent.  If it's currently silent,
                // do the opposite.
                RingerHelper.performToggle(audioManager);

                // Update the UI to reflect the new state
                updateUi();
            }
        });


        // This is how you log messages to adb logcat!
        Log.d("SilentModeToggle", "This is a test");
        //Log.d("intent", String.valueOf(notificationManager.isNotificationPolicyAccessGranted()));
    }


    /**
     * Updates the UI image to show an image representing silent or
     * normal, as appropriate
     */
    private void updateUi(){
        // Find the view named phone_icon in our layout.  We know it's
        // an ImageView in the layout, so downcast it to an ImageView.
        ImageView imageView = (ImageView) findViewById(R.id.phone_icon);

        // Set phoneImage to the ID of image that represents ringer on
        // or off.  These are found in res/mipmap-xxhdpi
        int phoneImage = RingerHelper.isPhoneSilent(audioManager)
                ? R.mipmap.ringer_off
                : R.mipmap.ringer_on;

        // Set the imageView to the image in phoneImage
        imageView.setImageResource(phoneImage);
    }

}
