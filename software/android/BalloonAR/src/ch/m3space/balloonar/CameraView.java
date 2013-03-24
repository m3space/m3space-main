package ch.m3space.balloonar;

import android.content.Context;
import android.hardware.Camera;
import android.util.AttributeSet;
import android.view.SurfaceHolder;
import android.view.SurfaceView;

public class CameraView extends SurfaceView implements SurfaceHolder.Callback {
	
	private SurfaceHolder holder;
	private Camera camera;

	public CameraView(Context context) {
		super(context);
		holder = this.getHolder();
		holder.addCallback(this);
	}
	
	public CameraView(Context context, AttributeSet attrs) {
		super(context, attrs);
		holder = this.getHolder();
		holder.addCallback(this);
	}
	
	public CameraView(Context context, AttributeSet attrs, int defStyle) {
		super(context, attrs, defStyle);
		holder = this.getHolder();
		holder.addCallback(this);
	}

	@Override
	public void surfaceCreated(SurfaceHolder holder) {
		camera = Camera.open();
		
		try {
			camera.setPreviewDisplay(holder);
		}
		catch (Exception e) {
			camera.release();
			camera = null;
		}
	}
	
	@Override
	public void surfaceChanged(SurfaceHolder holder, int format, int w, int h) {
		//Camera.Parameters parameters = camera.getParameters();
        //parameters.setPreviewSize(w, h);
        //camera.setParameters(parameters);
        camera.startPreview();
	}

	@Override
	public void surfaceDestroyed(SurfaceHolder holder) {
		camera.stopPreview();
		camera.release();
		camera = null;
	}

}
